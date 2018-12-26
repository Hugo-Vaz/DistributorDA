using DistDataAquisition.DAO;
using DistDataAquisition.Model;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistDataAquisition.Business
{
    public class DistributorReportBusiness
    {
        private DistributorReportDAO _dao;
        private DistributorDAO _distDAO;
        private List<Distibutor> _distibutors;
        public DistributorReportBusiness()
        {
            _dao = new DistributorReportDAO();
            _distDAO = new DistributorDAO();
            _distibutors = _distDAO.GetAll();
        }

        public void GenerateReport()
        {           
            List<DistributorReport> reports = _dao.ListByTimestamp(DateTime.Now.AddDays(-7),DateTime.Now);
            IWorkbook wb = new XSSFWorkbook();
            ISheet sheet = wb.CreateSheet(String.Format("Report {0}", DateTime.Now.ToString("dd-MM-YYYY")));
            IRow header = sheet.CreateRow(0);

            this.CreateHeader(header);

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

        private void CreateCell(IRow row,string content,int colNo)
        {
            ICell cell = row.CreateCell(colNo);
            cell.SetCellValue(content);
        }
    }
}
