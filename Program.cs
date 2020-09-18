using HtmlComparer.Model;
using HtmlComparer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            _compareService = new CompareService(
                AppConfigProvider.GetComparers(),
                AppConfigProvider.GetCheckers(),
                sources.First(x => x.CompareRole == CompareRole.Origin),
                sources.First(x => x.CompareRole == CompareRole.Target)
                );
        }

        private static async Task Start(IEnumerable<Page> pages)
        {
            NextAction next = NextAction.GoToNextPage;

            for (int i = 0; i < pages.Count(); i++)
            {
                var page = pages.ElementAt(i);
                do
                {
                    try
                    {
                        WritePageTitle(page, i + 1, pages.Count());
                        var report = await _compareService.GetReport(page, useCache: next != NextAction.RescanPrevious);
                        WriteReport(report);

                        if (next == NextAction.GoToNextErrorPage && !report.HasErrors)
                            continue;

                    }
                    catch (WebException ex)
                    {
                        WriteError($"\tWEB EXCEPTION: {ex.Message}\r\n");
                    }
                    catch (Exception ex)
                    {
                        WriteError($"\tFATAL ERROR: {ex.Message}\r\n");
                    }

                    next = NextActionQuestion();
                }
                while (next == NextAction.RescanPrevious);
            }
        }

        private static void WritePageTitle(Page page, int counter, int count)
        {
            var pageTitlePath = string.IsNullOrEmpty(page.Path) ? "<Root page>" : page.Path;
            var pageTitle = $"\r\nSource[{counter}/{count}]: {pageTitlePath}\r\n";

            Console.WriteLine($"{pageTitle}");
        }

        private static void WriteReport(Report report)
        {
            foreach (var r in report.Results)
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
                Console.WriteLine("Choose a display mode:\r\n\t1. Report on all pages\r\n\t2. Page to page report");
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

        private static NextAction NextActionQuestion()
        {
            if (_mode == DisplayMode.PageToPage)
            {
                Console.WriteLine("Page comparison done.\r\n" +
               "Rescan previous page - press 'p'\r\n" +
               "Go to the next error page - press 'n'\r\n" +
               "Go to the next page - press any other key\r\n");

                var keyChar = Console.ReadKey().Key.ToString().ToLower();
                switch (keyChar)
                {
                    case "p":
                    case "з": return NextAction.RescanPrevious;
                    case "n":
                    case "т": return NextAction.GoToNextErrorPage;
                    default: return NextAction.GoToNextPage;
                }
            }
            else
            {
                return NextAction.GoToNextPage;
            }
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

    enum DisplayMode
    {
        Undefined = 0,
        FullReport = 1,
        PageToPage = 2
    }
    enum NextAction
    {
        GoToNextPage,
        GoToNextErrorPage,
        RescanPrevious
    }
}
