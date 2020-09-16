using HtmlAgilityPack;
using System;
using System.Net;

namespace HtmlComparer
{
    public class PageResponse
    {
        public PageResponse(Uri requestedUri, HtmlDocument returnedHtmlDocument, HttpStatusCode statusCode, Uri returnedUri)
        {
            RequestedUri = requestedUri;
            ReturnedHtmlDocument = returnedHtmlDocument;
            StatusCode = statusCode;
            ReturnedUri = returnedUri;
        }

        public Uri RequestedUri { get; }
        public HtmlDocument ReturnedHtmlDocument { get; }
        public HttpStatusCode StatusCode { get; }
        public Uri ReturnedUri { get; }

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
