using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting scheduled task.");
            switch (args.First().ToLower())
            {
                case "generate_report":
                    Console.WriteLine("Generating weekly report");

                    DistDataAcquisition.Business.DistributorReportBusiness reportBusiness = new DistDataAcquisition.Business.DistributorReportBusiness();
                    reportBusiness.GenerateReport();
                    break;
            }
        }
    }
}
