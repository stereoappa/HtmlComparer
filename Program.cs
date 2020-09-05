using MetaComparer.Model;
using MetaComparer.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace MetaComparer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            List<Source> _sources = ConfigurationManager.GetSection("sources") as List<Source>;
            List<Page> _pages = ConfigurationManager.GetSection("pages") as List<Page>;
            List<ComparedField> _compareFields = ConfigurationManager.GetSection("compareFields") as List<ComparedField>;
            Console.WriteLine("Comparison started..");

            var _compareService = new CompareService(_sources);

            var fieldReport = await _compareService.CompareByFields(_pages, _compareFields);
            WriteReport(fieldReport, "-- FIELD COMPARER RESULT --");

            var urlReport = await _compareService.CheckReturnUrl(_pages);
            WriteReport(urlReport, "-- URL COMPARER RESULT --");

            var htlOutlineReport = await _compareService.CompareByHtmlOutline(_pages);
            WriteReport(htlOutlineReport, "-- HTML OUTLINE COMPARER RESULT --");

            Console.WriteLine("Done!");
            Console.ReadKey();
        }

        static void WriteReport(List<ICompareResult> report, string title = "")
        {
            if (!string.IsNullOrEmpty(title))
            {
                Console.WriteLine($"{title}\r\n");
            }
            report.ForEach(r => Console.WriteLine(r));
        }
    }
}
