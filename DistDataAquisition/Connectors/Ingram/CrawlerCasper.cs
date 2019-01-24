using DistDataAcquisition.DAO;
using DistDataAcquisition.Helpers;
using DistDataAcquisition.Model;
using DistDataAquisition.DAO;
using DistDataAquisition.Helpers;
using DistDataAquisition.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DistDataAcquisition.Connectors.Ingram
{
    public class CrawlerCasper
    {
        List<string> _expectedOrigins = new List<string> { "SP", "ES I", "ES II" };
        public void GetReports()
        {
            SKUDAO skuDao = new SKUDAO();
            DistributorReportDAO reportDAO = new DistributorReportDAO();
            DistributorDAO distributorDAO = new DistributorDAO();
            LogDAO logDAO = new LogDAO();
            List<DistributorReport> reports = new List<DistributorReport>();
            List<SKU> skus = skuDao.GetAll();
            Distributor distributor = distributorDAO.GetByName("Ingram");
            List<Log> logs = new List<Log>();
            DateTime timestamp = DateTime.Now;

            string[] parameters = new string[] { "ingramCrawl.js", string.Join(",", skus.Select(x => x.PartNumber)) };
            string json = CasperJSHelper.RunProcess(parameters, "Ingram_" + DateTime.Now.Millisecond.ToString() + ".json", "IngramCrawl");

            dynamic result = JsonConvert.DeserializeObject(json);

            if (!((bool)result.success))
            {
                logs.Add(new Log
                {
                    DistributorID = distributor.DistributorID,
                    Timestamp = DateTime.Now,
                    Message = String.Format(result.message)
                });
            }
            else
            {
                for(int i = 0, len = result.lines.Count; i < len; i++)
                {
                    if (!this.ValidEntry(result.lines[i].origin.ToString())) continue;

                    var sku = skus.Where(x => x.PartNumber.Equals(result.lines[i].sku.ToString())).FirstOrDefault();
                    try
                    {
                        DistributorReport currentReport = new DistributorReport();
                        currentReport.DistributorID = distributor.DistributorID;
                        currentReport.SKUID = sku.SKUID;
                        currentReport.Origin = result.lines[i].origin.ToString();
                        currentReport.SellingPrice = GenericHelper.ConvertToDecimal(result.lines[i].sellingPrice.ToString(), "SellingPrice", false);
                        currentReport.StockQty = (string.IsNullOrEmpty(result.lines[i].stockQty.ToString())) ? null : result.lines[i].stockQty;
                        currentReport.Timestamp = timestamp;
                        reportDAO.Save(currentReport);
                        reports.Add(currentReport);
                    }catch(Exception e)
                    {
                        var log = new Log
                        {
                            DistributorID = distributor.DistributorID,
                            SKUID = sku.SKUID,
                            Timestamp = DateTime.Now,
                            Message = String.Format("The following SKU ({0}) was not processed for {1}. Exception: {2}", sku.PartNumber, distributor.Name, e.Message)
                        };

                        logDAO.Save(log);
                        logs.Add(log);
                    }
                }
            }


        }

        private bool ValidEntry(string origin)
        {
            return _expectedOrigins.Contains(origin) || string.IsNullOrEmpty(origin);
        }
    }
}
