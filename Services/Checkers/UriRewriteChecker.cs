using HtmlComparer.Model;
using System;

namespace HtmlComparer.Services.Checkers
{
    public class UriRewriteChecker : IPageChecker
    {
        public IReportRow Check(PageResponse page)
        {
            var res = page.RequestedUri.LocalPath.ToLower() == page.ReturnedUri.LocalPath;

            return new UriRewriteCheckerResult(page, res);
        }
    }

    class UriRewriteCheckerResult : IReportRow
    {
        private PageResponse _page;

        public bool HasErrors { get; }
        public Uri PageUri => _page.RequestedUri;

        public UriRewriteCheckerResult(PageResponse page, bool uriIsEquals)
        {
            _page = page;
            HasErrors = !uriIsEquals;
        }

        public override string ToString()
        {
            string res = $"\tREWRITE CHECKER: ";
            if (HasErrors)
            {
                res += "\r\n\tERROR: Return Uri " +
                        _page.ReturnedUri.Host +
                        _page.ReturnedUri.PathAndQuery +
                        _page.ReturnedUri.Fragment +
                        " is incorrect \r\n";
                return res;
            }
            else
            {
                res += $"OK: Returned Url is in lower case. Host: {PageUri.Host}.\r\n";
                return res;
            }


        }
    }
}
