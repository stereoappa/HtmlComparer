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
            var sources = AppConfigProvider.GetSource();
            var pages = AppConfigProvider.GetPages();
            var compareTags = AppConfigProvider.GetCompareTags();

            var comparers = new List<IPagesComparer> { new TagComparer(compareTags), new HtmlOutlineComparer() };
            var compareService = new CompareService(
                comparers, 
                sources.First(x => x.CompareRole == CompareRole.Origin),
                sources.First(x => x.CompareRole == CompareRole.Target)
                );

            Console.WriteLine("Comparison started..");

            IEnumerable<ICompareResult> comparerReport = null;
            try
            {
                foreach (var page in pages)
                {
                    comparerReport = await compareService.Compare(page);
                    var withRewriteRuleResults = await compareService.CheckRewriteRule(page);
                    comparerReport = comparerReport.Union(withRewriteRuleResults);
                    WriteReport(comparerReport);
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FATAL ERROR: {ex.Message}");
            }

            Console.WriteLine("Html compare had done!");
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
