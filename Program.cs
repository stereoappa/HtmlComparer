using HtmlComparer.Model;
using HtmlComparer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlComparer.Comparers;

namespace HtmlComparer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var sources = ConfigProvider.GetSource();
            var pages = ConfigProvider.GetPages();
            var compareTags = ConfigProvider.GetCompareTags();

            var comparers = new List<IPageComparer>
            {
                new TagComparer(compareTags),
                new HtmlOutlineComparer()
            };
            var compareService = new CompareService(comparers, sources, pages);

            Console.WriteLine("Comparison started..");

            IEnumerable<ICompareResult> comparerResults = null;
            try
            {
                comparerResults = await compareService.Compare();
                var withRewriteRuleResults = await compareService.CheckRewriteRule();
                comparerResults = comparerResults.Union(withRewriteRuleResults);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FATAL ERROR: {ex.Message}");
            }

            WriteReport(comparerResults);

            Console.WriteLine("Html compared is done!");
            Console.ReadKey();
        }

        static void WriteReport(IEnumerable<ICompareResult> reports)
        {

            foreach (var group in reports.GroupBy(x => x.OriginPage.LocalPath.ToLower()))
            {
                var resultForSource = group.Select(g => g);
                WriteCompareResults(resultForSource, $"Source: {group.Key}");
            }
        }

        static void WriteCompareResults(IEnumerable<ICompareResult> results, string title)
        {
            if (!string.IsNullOrEmpty(title))
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{title}\r\n");
            }

            foreach (var r in results)
            {
                Console.ForegroundColor = r.HasErrors ? ConsoleColor.Red : ConsoleColor.White;
                Console.WriteLine(r.ToString(), ConsoleColor.Red);
            }
        }
    }
}
