using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistDataAcquisition.Business
{
    public class NetworkBusiness
    {
        public void GetData()
        {
            Connectors.Network1.Crawler crawler = new Connectors.Network1.Crawler();
            crawler.GetReport();
        }
    }
}
