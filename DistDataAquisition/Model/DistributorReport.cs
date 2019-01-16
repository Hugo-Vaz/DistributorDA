using System;

namespace DistDataAcquisition.Model
{
    public class DistributorReport
    {
        [Identity]
        public int DistributorReportID { get; set; }
        public int DistributorID { get; set; }
        public int SKUID { get; set; }

        [Identity]
        private string _origin { get; set; }
        public string Origin { get {
                return string.IsNullOrEmpty(_origin) ? "Not informed" : _origin;
            }
            set {
                _origin = value;
            }
        }
        public string StockQty { get; set; }
        public decimal? SellingPrice { get; set; }
        public DateTime Timestamp { get; set; } 

        [AnotherObject]
        public SKU SKU { get; set; }
        [AnotherObject]
        public Distributor Distributor { get; set; }
    }
}
