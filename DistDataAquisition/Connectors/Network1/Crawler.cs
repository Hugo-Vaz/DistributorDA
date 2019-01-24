using DistDataAcquisition.DAO;
using DistDataAcquisition.Model;
using DistDataAquisition.DAO;
using DistDataAquisition.Helpers;
using DistDataAquisition.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WatiN.Core;

namespace DistDataAcquisition.Connectors.Network1
{
    public class Crawler
    {
        public List<DistributorReport> GetReport()
        {
            Exception exception = null;
            SKUDAO skuDao = new SKUDAO();
            DistributorDAO distributorDAO = new DistributorDAO();
            List<DistributorReport> reports = new List<DistributorReport>();
            List<Log> logs= new List<Log>();

            List<SKU> skus = skuDao.GetAll();
            Distributor distributor = distributorDAO.GetByName("Network1");//new Distributor() { ResellerName = "TIMIX", UserName = "WAGNER", Password = "Joana2305" };

            System.Threading.Thread thread = new Thread(new ParameterizedThreadStart(s => Start(ref reports, ref logs, skus, distributor)));
            thread.SetApartmentState(ApartmentState.STA);

            thread.Start();
            thread.Join();

            if (exception != null)
            {
                throw exception;
            }

            return reports;
        }

        private void Start(ref List<DistributorReport> reports, ref List<Log> logs, List<SKU> skus, Distributor distributor)
        {
            IE browser = null;
            DistributorReportDAO reportDAO = new DistributorReportDAO();
            LogDAO logDAO = new LogDAO();
            DateTime timestamp = DateTime.Now;

            browser = IEBrowserHelper.GetBrowser();
            browser.GoTo("http://www.network1.com.br/lojavirtual");
            browser.WaitForComplete();

            browser.TextField(Find.ByName("revenda")).SetAttributeValue("value", distributor.ResellerName);
            browser.TextField(Find.ByName("ID")).SetAttributeValue("value",distributor.UserName);
            browser.TextField(Find.ByName("SENHA")).SetAttributeValue("value", distributor.Password);
            browser.Form(Find.ByName("frmLogin")).Submit();

            string url = browser.Url;
            while (!url.Equals("http://nbc.intersmartweb.com.br/nbc/vitrine.asp?Pesq=S")) { url = browser.Url; }

            for(int i=0,len = skus.Count; i < len; i++)
            {
                try
                {
                    browser.TextField(Find.ByName("txtBusca")).SetAttributeValue("value", skus[i].PartNumber);
                    browser.Element(Find.ByName("image2")).Click();

                    browser.WaitForComplete();
                    reports.AddRange(this.GetSearchResult(browser, skus[i],distributor,timestamp));
                }
                catch (Exception e)
                {
                    logs.Add(new Log
                    {
                        DistributorID = distributor.DistributorID,
                        SKUID = skus[i].SKUID,
                        Timestamp = DateTime.Now,
                        Message = String.Format("The following SKU ({0}) was not processed for {1}. Exception: {2}", skus[i].PartNumber, distributor.Name, e.Message)
                    });                    
                }
            }

          
           if(reports.Count > 0)  reportDAO.Save(reports);
           if (logs.Count > 0) logDAO.Save(logs);
        }

        private List<DistributorReport> GetSearchResult(IE browser, SKU sku,Distributor distributor,DateTime timestamp)
        {
            List<DistributorReport> reports = new List<DistributorReport>();
            DistributorReport currentReport;
            var table = browser.Table(Find.ById("tabela_det"));

            if(!table.Text.Contains("Nenhum produto encontrado"))
            {
                var fonts = browser.ElementsWithTag("FONT").Where(x => x.Style.Color.Equals("#990000")).ToList();
                for(int itemIndex =0,itemLen = fonts.Count;itemIndex < itemLen; itemIndex += 2)
                {
                    currentReport = new DistributorReport();
                    string[] data = fonts[itemIndex].Text.Split('\n');
                    currentReport.SKUID = sku.SKUID;
                    currentReport.SellingPrice = GenericHelper.ConvertToDecimal(data.First().Replace(" ", ""), "Selling Price", false);
                    currentReport.Origin = GenericHelper.CleanString(data[1].Trim().Split(' ')[1]);
                    var qtyElement = fonts[itemIndex].Parent.NextSibling;
                    while (qtyElement.ClassName == null || !qtyElement.ClassName.Equals("texto"))
                    {
                        qtyElement = qtyElement.NextSibling;
                    }

                    currentReport.StockQty = GenericHelper.CleanString(qtyElement.Text.Split('\n').FirstOrDefault());
                    currentReport.Timestamp = timestamp;
                    currentReport.DistributorID = distributor.DistributorID;

                    reports.Add(currentReport);
                }
            }
            else
            {
                currentReport = new DistributorReport();
                currentReport.SKUID = sku.SKUID;
                currentReport.StockQty = null;
                currentReport.SellingPrice = null;
                currentReport.Origin = null;
                currentReport.Timestamp = timestamp;
                currentReport.DistributorID = distributor.DistributorID;

                reports.Add(currentReport);
            }

            return reports;
        }
    }
}
