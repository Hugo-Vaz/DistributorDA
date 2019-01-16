using DistDataAcquisition.DAO;
using DistDataAcquisition.Helpers;
using DistDataAcquisition.Model;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistDataAcquisition.Business
{
    public class DistributorReportBusiness
    {
        private readonly DistributorReportDAO _dao;
        private readonly DistributorDAO _distDAO;
        private readonly UserDAO _userDAO;
        private List<Distributor> _distibutors;
        private string _tempDirectory = ConfigurationManager.AppSettings["TempFile"].ToString();

        public DistributorReportBusiness()
        {
            _dao = new DistributorReportDAO();
            _distDAO = new DistributorDAO();
            _userDAO = new UserDAO();
            _distibutors = _distDAO.GetAll();
        }

        public void GenerateReport()
        {           
            List<DistributorReport> reports = _dao.ListByTimestamp(DateTime.Now.AddDays(-7),DateTime.Now);
            List<User> users = _userDAO.GetUsersForReport();
            string fileName = string.Format("//DistributorReport_{0}.xlsx", DateTime.Now.ToString("ddMMyyyy-HHmm")),
                filePath = _tempDirectory + fileName;

            if (File.Exists(filePath))
                File.Delete(filePath);

            using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                IWorkbook wb = new XSSFWorkbook(stream);
                ISheet sheet = wb.CreateSheet(String.Format("Report {0}", DateTime.Now.ToString("dd-MM-YYYY")));
                IRow header = sheet.CreateRow(0);

                this.CreateHeader(header);
                this.CreateBody(sheet, reports);

                EmailSender.SendReport(users.Select(x => x.Email).ToList(), stream, fileName);
            }
            try
            {
                File.Delete(filePath);
            }
            catch { }
        }

        private void CreateHeader(IRow row)
        {
            int cellNo = 0;
            this.CreateCell(row, "SKU", cellNo);
            this.CreateCell(row, "Origem", ++cellNo);
            foreach(var dist in _distibutors)
            {
                this.CreateCell(row, dist.Name + " - Preço", ++cellNo);
                this.CreateCell(row, dist.Name + " - Estoque", ++cellNo);
            }
        }

        private void CreateBody(ISheet sheet,List<DistributorReport> reports)
        {
            var grouping = reports.Select(x => new { SKUID = x.SKUID, Origin = x.Origin,PartNumber = x.SKU.PartNumber }).Distinct().ToList();
            int rowNo = 1;
            foreach(var currentGroup in grouping)
            {
                IRow current = sheet.CreateRow(rowNo);
                var rowReports = reports.Where(x => x.SKUID.Equals(currentGroup.Origin) && x.Origin.Equals(currentGroup.Origin)).ToList();
                int colNo = 0;
                this.CreateCell(current, currentGroup.PartNumber, colNo);
                this.CreateCell(current, currentGroup.Origin, ++colNo);

                foreach (var dist in _distibutors)
                {
                    var distReport = rowReports.Where(x => x.DistributorID.Equals(dist.DistibutorID)).FirstOrDefault();
                    if (distReport == null)
                    {
                        this.CreateCell(current, " - ", ++colNo);
                        this.CreateCell(current, " - ", ++colNo);

                    }
                    else
                    {
                        this.CreateCell(current, Convert.ToDouble(distReport.SellingPrice), ++colNo);
                        this.CreateCell(current, distReport.StockQty, ++colNo);
                    }
                }
            }
        }

        private void CreateCell(IRow row,string content,int colNo)
        {
            ICell cell = row.CreateCell(colNo);
            cell.SetCellValue(content);
        }

        private void CreateCell(IRow row, double content, int colNo)
        {
            ICell cell = row.CreateCell(colNo);
            cell.SetCellValue(content);
        }

        private void CreateCell(IRow row, int content, int colNo)
        {
            ICell cell = row.CreateCell(colNo);
            cell.SetCellValue(content);
        }
    }
}
