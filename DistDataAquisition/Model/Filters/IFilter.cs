using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistDataAquisition.Model.Filters
{
    public interface IFilter
    {
        int last_page { get; set; }
        int current_page { get; set; }
        int per_page { get; set; }
        int from { get; set; }
        int to { get; set; }
        Decimal total { get; set; }
    }
}
