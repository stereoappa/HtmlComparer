using HtmlComparer.Model;
using System;

namespace HtmlComparer.Comparers
{
    public class UriRewriteChecker
    {
        public ICompareResult Compare(PageResponse target)
        {
            var res = target.RequestedUri.LocalPath.ToLower() == target.ReturnedUri.LocalPath;

            return new UriRewriteCheckerResult(target, res);
        }
    }

    public class UriRewriteCheckerResult : ICompareResult
    {
        private PageResponse _page;

        public bool HasErrors { get; }
        public Uri OriginPage => _page.RequestedUri;

        public UriRewriteCheckerResult(PageResponse page, bool uriIsEquals)
        {
            _page = page;
            HasErrors = !uriIsEquals;
        }

        public override string ToString()
        {
            string res = $"\tURI COMPARER: ";
            if (HasErrors)
            {
                res += $"\r\n\tERROR: Return Uri is incorrect {_page.RequestedUri.LocalPath}\r\n";
                return res;
            }
            else
            {
                res += "OK: Return Url is lower case\r\n";
                return res;
            }


        }
    }
}
