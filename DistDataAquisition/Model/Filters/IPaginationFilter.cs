using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistDataAquisition.Model.Filters
{
    public interface IPaginationFilter
    {
        int page { get; set; }
        int per_page { get; set; }

        bool sort_ascending { get; set; }

        string order_by { get; set; }

    }
}
