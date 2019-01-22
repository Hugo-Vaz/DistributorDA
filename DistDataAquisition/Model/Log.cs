using DistDataAcquisition;
using DistDataAcquisition.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistDataAquisition.Model
{
    public class Log
    {
        [Identity]
        public Int32 LogID { get; set; }

        public Int32 SKUID { get; set; }

        public Int32 DistributorID { get; set; }

        public string Message { get; set; }
        public DateTime Timestamp { get; set; }

        [AnotherObject]
        public SKU SKU { get; set; }
        [AnotherObject]
        public Distributor Distributor { get; set; }
    }
}
