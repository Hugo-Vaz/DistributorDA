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
            Connectors.Ingram.Crawler crawler = new Connectors.Ingram.Crawler();
            crawler.GetReport();
        }
    }
}
