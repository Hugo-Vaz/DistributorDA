using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistDataAcquisition.Business
{
    public class IngramBusiness
    {
        public void GetData()
        {
            Connectors.Ingram.CrawlerCasper crawler = new Connectors.Ingram.CrawlerCasper();
            crawler.GetReports();
        }
    }
}
