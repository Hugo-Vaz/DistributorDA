using DistDataAcquisition.DAO;
using DistDataAcquisition.Helpers;
using DistDataAcquisition.Model;
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
        public void GetReports()
        {
            SKUDAO skuDao = new SKUDAO();
            DistributorDAO distributorDAO = new DistributorDAO();
            List<DistributorReport> reports = new List<DistributorReport>();
            List<SKU> skus = new List<SKU> { new SKU { PartNumber = "WS-C2960X-48TD-L" }, new SKU { PartNumber = "ASA5506-K8-BR" } }; //skuDao.GetAll();
            Distributor distributor =/* distributorDAO.GetByName("Ingram");*/ new Distributor() { UserName = "Admin10991602", Password = "Joana230517" };
            List<Log> logs = new List<Log>();

            string[] parameters = new string[] { "ingramCrawl.js", string.Join(",", skus.Select(x => x.PartNumber)) };
            string json = CasperJSHelper.RunProcess(parameters, "Ingram_" + DateTime.Now.Millisecond.ToString() + ".json", "IngramCrawl");

            dynamic result = JsonConvert.DeserializeObject(json);

            if (!((bool)result.success))
            {
                logs.Add(new Log
                {
                    DistributorID = distributor.DistibutorID,
                    Timestamp = DateTime.Now,
                    Message = String.Format(result.message)
                });
            }
            else
            {
                for(int i = 0, len = result.lines.Count; i < len; i++)
                {
                    var sku = skus.Where(x => x.PartNumber.Equals(result.lines[i].sku.ToString())).FirstOrDefault();
                    try
                    {
                        DistributorReport currentReport = new DistributorReport();
                        currentReport.DistributorID = distributor.DistibutorID;
                        currentReport.SKUID = sku.SKUID;
                        currentReport.Origin = result.lines[i].origin.ToString();
                        currentReport.SellingPrice = GenericHelper.ConvertToDecimal(result.lines[i].sellingPrice.ToString(), "SellingPrice", false);
                        currentReport.StockQty = (string.IsNullOrEmpty(result.lines[i].stockQty.ToString())) ? null : result.lines[i].stockQty;

                        reports.Add(currentReport);
                    }catch(Exception e)
                    {
                        logs.Add(new Log
                        {
                            DistributorID = distributor.DistibutorID,
                            SKUID = sku.SKUID,
                            Timestamp = DateTime.Now,
                            Message = String.Format("The following SKU ({0}) was not processed for {1}. Exception: {2}", sku.PartNumber, distributor.Name, e.Message)
                        });
                    }
                }
            }
        }
    }
}
