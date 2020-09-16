using HtmlAgilityPack;
using System;
using System.Net;

namespace HtmlComparer
{
    public class PageResponse
    {
        public Uri ReturnedUri { get; set; }
        public Uri RequestedUri { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public HtmlDocument ReturnedHtmlDocument { get; set; }

        public void ThrowIfPageNotFound()
        {
            if (StatusCode != HttpStatusCode.OK)
            {
                throw new WebException($"The page {RequestedUri} is not available.\r\n\tStatus code: {(int)StatusCode}. The page is not accepted for comparison");
            }
        }

        public HtmlNodeCollection FindNodesByXpath(string xpath)
        {
            return ReturnedHtmlDocument?.DocumentNode.SelectNodes(xpath);
        }   
    }
}
