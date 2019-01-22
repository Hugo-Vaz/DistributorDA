using DistDataAcquisition.DAO;
using DistDataAcquisition.Model;
using DistDataAquisition.Helpers;
using DistDataAquisition.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WatiN.Core;

namespace DistDataAcquisition.Connectors.Ingram
{
    public class Crawler
    {
        public List<DistributorReport> GetReport()
        {
            Exception exception = null;
            SKUDAO skuDao = new SKUDAO();
            DistributorDAO distributorDAO = new DistributorDAO();
            List<DistributorReport> reports = new List<DistributorReport>();
            List<SKU> skus = new List<SKU> { new SKU { PartNumber = "WS-C2960X-48TD-L" }, new SKU { PartNumber = "ASA5506-K8-BR" } }; //skuDao.GetAll();
            Distributor distributor =/* distributorDAO.GetByName("Ingram");*/ new Distributor() { UserName = "Admin10991602", Password = "Joana230517" };

            System.Threading.Thread thread = new Thread(new ParameterizedThreadStart(s => Start(ref reports, ref exception, skus, distributor)));
            thread.SetApartmentState(ApartmentState.STA);

            thread.Start();
            thread.Join();

            if (exception != null)
            {
                throw exception;
            }

            return reports;
        }

        private void Start(ref List<DistributorReport> reports, ref Exception exception, List<SKU> skus, Distributor distributor)
        {
            IE browser = null;
            DistributorReportDAO reportDAO = new DistributorReportDAO();

            browser = new IE();
            browser.GoTo("https://br.ingrammicro.com/_layouts/CommerceServer/IM/Login.aspx?returnurl=//br.ingrammicro.com/");
            browser.WaitForComplete();

            browser.TextField(Find.ById("ctl00_PlaceHolderMain_txtUserEmail")).SetAttributeValue("value", distributor.UserName);
            browser.TextField(Find.ById("ctl00_PlaceHolderMain_txtPassword")).SetAttributeValue("value", distributor.Password);
            browser.Element(Find.ById("ctl00_PlaceHolderMain_btnLogin")).Click();

            string url = browser.Url;
            while (!url.Equals("https://br.ingrammicro.com/")) { url = browser.Url; }
            browser.WaitForComplete();

            for (int i = 0, len = skus.Count; i < len; i++)
            {
                try
                {
                    var search = browser.TextField(Find.ByClass("search-text"));
                    search.SetAttributeValue("value", skus[i].PartNumber);

                    var click = browser.Element(Find.ByClass("search-submit"));
                    click.Click();                    

                    browser.WaitForComplete();
                    reports.AddRange(this.GetSearchResult(browser, skus[i]));
                }
                catch (Exception e)
                {
                    var log = new Log
                    {
                        DistributorID = distributor.DistibutorID,
                        SKUID = skus[i].SKUID,
                        Timestamp = DateTime.Now,
                        Message = String.Format("The following SKU ({0}) was not processed for {1}. Exception: {2}", skus[i].PartNumber, distributor.Name, e.Message)
                    };

                    //save log
                }
            }

            foreach (var report in reports)
            {
                reportDAO.Save(report);
            }

        }

        private List<DistributorReport> GetSearchResult(IE browser, SKU sku)
        {
            List<DistributorReport> reports = new List<DistributorReport>();
            DistributorReport currentReport;
            var elements = browser.Divs.Where(x=>x.ClassName != null && x.ClassName.Contains("Warehousetooltip")).ToList();

            if (elements != null && elements.Count > 0)
            {
               
                for (int itemIndex = 0, itemLen = elements.Count; itemIndex < itemLen; itemIndex++)
                {
                    currentReport = new DistributorReport();
                    currentReport.SKU = sku;
                    int childCount = elements[itemIndex].Elements.Count;

                    string price = elements[itemIndex].Elements[childCount - 1].Text.Trim();
                    if (price.Contains("Sob Consulta"))
                        currentReport.SellingPrice = null;
                    else
                        currentReport.SellingPrice = GenericHelper.ConvertToDecimal(price, "Selling price");

                    currentReport.StockQty = GenericHelper.CleanString(elements[itemIndex].Elements[childCount - 2].Text);
                    currentReport.Origin = GenericHelper.CleanString(elements[itemIndex].Elements[childCount - 3].Text.Replace(":",""));

                    reports.Add(currentReport);
                }
            }
            else
            {
                currentReport = new DistributorReport();
                currentReport.SKU = sku;
                currentReport.StockQty = null;
                currentReport.SellingPrice = null;
                currentReport.Origin = null;

                reports.Add(currentReport);
            }

            return reports;
        }
    }
}
