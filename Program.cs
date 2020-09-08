using HtmlComparer.Model;
using HtmlComparer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlComparer.Comparers;
using System.Net;

namespace HtmlComparer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Start();
        }

        private static async Task Start()
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
            foreach (var page in pages)
            {
                try
                {
                    WriteReport(await compareService.GetCompareReport(page));
                }
                catch (WebException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Source: {page.Path}\r\n\tFATAL ERROR: {ex.Message}");
                    Console.ForegroundColor = ConsoleColor.White;
                }         
            }

            Console.WriteLine("\r\nHtml comparison done!\r\nIf you want to rescan all pages press 'r'");
            await AskRebootComparisonQuestion();
        }     

        private static void WriteReport(IEnumerable<IGrouping<string, ICompareResult>> report)
        {
            foreach (var group in report)
            {
                var resultForSource = group.Select(g => g);
                WriteCompareResults(resultForSource, $"Source: {group.Key}");
            }
        }

        private static void WriteCompareResults(IEnumerable<ICompareResult> results, string title)
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

        private static async Task AskRebootComparisonQuestion()
        {
            var key = Console.ReadKey();
            if (key.KeyChar.ToString().ToLower() == "r" || key.KeyChar.ToString().ToLower() == "к")
            {
                Console.Clear();
                await Start();
                Console.ReadKey();
            }
        }
    }
}
