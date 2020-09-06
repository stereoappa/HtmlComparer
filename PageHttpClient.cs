using HtmlAgilityPack;
using HtmlComparer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HtmlComparer.Model
{
    public class PageHttpClient
    {
        private HtmlWeb _web;
        private TaskFactory<HtmlDocument> _factory;
        private Dictionary<Uri, PageResponse> _cache;

        public PageHttpClient()
        {
            _web = new HtmlWeb();
            _factory = new TaskFactory<HtmlDocument>();
            _cache = new Dictionary<Uri, PageResponse>();

        }

        public async Task<PageResponse> GetResponse(Uri uri)
        {
            if(_cache.ContainsKey(uri))
            {
                return _cache[uri];
            }

            var doc = await _factory.StartNew(() => _web.Load(uri.ToString()));

            if (doc == null)
            {
                throw new HttpException(404, "Page not found");
            }


            var response = new PageResponse
            {
                RequestedUri = uri,
                ReturnedHtmlDocument = doc,
                ReturnedUri = _web.ResponseUri
            };

            _cache.Add(uri, response);
            return response;
        }

        public async Task<PageResponse> GetResponse(string baseUrl, string path)
        {
            return await GetResponse(BuildPath(baseUrl, path));
        }

        private Uri BuildPath(string domain, string path)
        {
            return new Uri(new Uri(domain), path);
        }
    }
}
