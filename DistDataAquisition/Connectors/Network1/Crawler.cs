using DistDataAcquisition.DAO;
using DistDataAcquisition.Model;
using DistDataAquisition.Helpers;
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
            List<DistributorReport> reports = new List<DistributorReport>();
            //List<SKU> skus = skuDao.GetAll();
            Distributor distributor = new Distributor() { ResellerName = "TIMIX", UserName = "WAGNER", Password = "Joana2305" };
            System.Threading.Thread thread = new Thread(new ParameterizedThreadStart(s => Start(ref reports, ref exception, new List<SKU> { new SKU { PartNumber = "AIRCAP1702I-ZK9BR=" } }, distributor)));
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
            try
            {
                browser = new IE();
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
                    browser.TextField(Find.ByName("txtBusca")).SetAttributeValue("value", skus[i].PartNumber);
                    browser.Element(Find.ByName("image2")).Click();

                    browser.WaitForComplete();
                    reports.AddRange(this.GetSearchResult(browser, skus[i]));
                }

            }
            catch(Exception e)
            {

            }
        }

        private List<DistributorReport> GetSearchResult(IE browser, SKU sku)
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
                    currentReport.SKU = sku;

                    var qtyElement = fonts[itemIndex].Parent.NextSibling;
                    while (qtyElement.ClassName.Equals("texto"))
                    {
                        qtyElement = qtyElement.NextSibling;
                    }

                    currentReport.StockQty = qtyElement.Text.Split('\n').FirstOrDefault();
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
