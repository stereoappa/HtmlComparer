using HtmlComparer.Model;
using HtmlComparer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlComparer.Services.Comparers;
using System.Net;
using HtmlComparer.Configuration;

namespace HtmlComparer
{
    class Program
    {
        private static DisplayMode _mode = DisplayMode.Undefined;
        private static CompareService _compareService;
        private static IEnumerable<Page> _pages;
        static async Task Main(string[] args)
        {
            do
            {
                Console.Clear();
                Init(args);
                Console.Clear();

                Console.WriteLine("Comparison start..");
                await Start(_pages);
            }
            while (RebootFullComparisonQuestion());
        }

        private static void Init(string[] args)
        {
            _mode = DisplayModeQuestion(args);
            _pages = AppConfigProvider.GetPages();
            var sources = AppConfigProvider.GetSource();
            var compareTags = AppConfigProvider.GetCompareTags();

            var comparers = new List<IPagesComparer> { new TagComparer(compareTags), new HtmlOutlineComparer() };
            _compareService = new CompareService(
                comparers,
                sources.First(x => x.CompareRole == CompareRole.Origin),
                sources.First(x => x.CompareRole == CompareRole.Target)
                );
        }

        private static async Task Start(IEnumerable<Page> pages)
        {
            NextAction nextDoing = NextAction.GoToNextPage;

            for (int i = 0; i < pages.Count(); i++)
            {
                var page = pages.ElementAt(i);                
                do
                {
                    try
                    {
                        WritePageTitle(page, i + 1, pages.Count());
                        WriteReport(await _compareService.GetReport(page, useCache: nextDoing == NextAction.RescanPrevious));
                    }
                    catch (WebException ex)
                    {
                        WriteError($"\tWEB EXCEPTION: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        WriteError($"\tFATAL ERROR: {ex.Message}");
                    }
                    finally
                    {
                        nextDoing = NextDoingQuestion();  
                    }
                }
                while (nextDoing == NextAction.RescanPrevious);
            }
        }

        private static void WritePageTitle(Page page, int counter, int count)
        {
            var pageTitlePath = string.IsNullOrEmpty(page.Path) ? "<Root page>" : page.Path;
            var pageTitle = $"\r\nSource[{counter}/{count}]: {pageTitlePath}\r\n";
          
            Console.WriteLine($"{pageTitle}");       
        }

        private static void WriteReport(IEnumerable<IGrouping<string, ICompareResult>> report)
        {
            foreach (var group in report)
            {
                var resultForSource = group.Select(g => g);
                WriteComparerResults(resultForSource);
            }
        }

        private static void WriteComparerResults(IEnumerable<ICompareResult> results)
        {
            foreach (var r in results)
            {
                if (r.HasErrors)
                {
                    WriteError(r.ToString());
                }
                else
                {
                    Console.WriteLine(r.ToString());
                }
            }
        }

        private static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static DisplayMode DisplayModeQuestion(string[] args = null)
        {
            if (_mode != DisplayMode.Undefined)
            {
                return _mode;
            }

            string dm;
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Choose display mode:\r\n\t1. Full report by all pages\r\n\t2. Page to page report");
                dm = Console.ReadKey().KeyChar.ToString();
            }
            else
            {
                dm = args
                    .FirstOrDefault(x => x.Contains("displaymode"))?
                    .Split('=')
                    .LastOrDefault();
            }

            Enum.TryParse(dm, out DisplayMode mode);
            return mode;
        }

        private static NextAction NextDoingQuestion()
        {
            Console.WriteLine("Page comparison done.\r\n");
            NextAction res = NextAction.GoToNextPage;

            if (_mode == DisplayMode.PageByPage)
            {
                Console.WriteLine("Rescan previous page - press 'p'\r\n" +
                    "Go to next page - press any other key\r\n" +
                    "Go to next error page - press 'n'\r\n");

                var keyChar = Console.ReadKey().Key;
                res = keyChar.ToString().ToLower() == "p" ||
                    keyChar.ToString().ToLower() == "з" ? NextAction.RescanPrevious :
                    keyChar.ToString().ToLower() == "n" ||
                    keyChar.ToString().ToLower() == "т" ? NextAction.GoToNextErrorPage : 
                    NextAction.GoToNextPage;
            }
            
            return res;
        }

        private static bool RebootFullComparisonQuestion()
        {
            Console.WriteLine("All pages comparison done!\r\n" +
                "Rescan all pages - press 'r'");

            var keyChar = Console.ReadKey().Key;
            var answer = keyChar.ToString().ToLower() == "r" ||
                keyChar.ToString().ToLower() == "к" ? true : false;

            return answer;
        }
    }

    public enum DisplayMode
    {
        Undefined = 0,
        FullReport = 1,
        PageByPage = 2,
        ToNextError = 3
    }
    public enum NextAction
    {
        RescanPrevious,
        GoToNextPage,
        GoToNextErrorPage
    }
}
