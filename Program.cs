using HtmlComparer.Model;
using HtmlComparer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using HtmlComparer.Infrastructure.Data;

namespace HtmlComparer
{
    class Program
    {
        private static AppConfigModelProvider _config;
        private static CompareService _compareService;
        private static List<Page> _pages;
        
        static async Task Main(string[] args)
        {
            do
            {
                Console.Clear();
                Init(args);
                var mode = DisplayModeQuestion(args);
                Console.Clear();

                await Start(_pages, mode);
            }
            while (RebootFullComparisonQuestion());
        }

        private static void Init(string[] args)
        {
            _config = new AppConfigModelProvider();
            _pages = _config.GetPages().ToList();
            _config.GetCustomPageProviders()
                   .ToList()
                   .ForEach(x => _pages.AddRange(x.GetPages())); 
            
            var sources = _config.GetSource();

            _compareService = new CompareService(
                _config.GetComparers(),
                _config.GetCheckers(),
                sources.First(x => x.CompareRole == CompareRole.Origin),
                sources.First(x => x.CompareRole == CompareRole.Target)
                );
        }

        private static async Task Start(IEnumerable<Page> pages, DisplayMode mode)
        {
            if (mode == DisplayMode.ViewPageList)
            {
                WritePageList(pages);
                return;
            }

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

                    next = GetNextAction(mode);
                }
                while (next == NextAction.RescanPrevious);
            }
        }

        private static void WritePageTitle(Page page, int counter, int count)
        {
            var pageTitlePath = string.IsNullOrEmpty(page.LocalPath) ? "<Root page>" : page.LocalPath;
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

        private static void WritePageList(IEnumerable<Page> pages)
        {
            for (int i = 0; i < pages.Count(); i++)
            {
                var page = pages.ElementAt(i);
                var pageTitlePath = string.IsNullOrEmpty(page.LocalPath) ? "<Root page>" : page.LocalPath;
                Console.WriteLine((i + 1) + ". " + pageTitlePath + "\r\n");
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
            string dm;
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Choose a display mode:" +
                    "\r\n\t1. Report on all pages" +
                    "\r\n\t2. Page to page report" +
                    "\r\n\t3. View page list");
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

        private static NextAction GetNextAction(DisplayMode mode)
        {
            if (mode == DisplayMode.PageToPage)
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
            Console.WriteLine("Completed successfully!\r\n" +
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
        PageToPage = 2,
        ViewPageList = 3
    }
    enum NextAction
    {
        GoToNextPage,
        GoToNextErrorPage,
        RescanPrevious
    }
}
