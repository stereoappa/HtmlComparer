﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HtmlComparer
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

        public async Task<PageResponse> GetResponse(Uri uri, bool useCache = true)
        {
            if (useCache && _cache.ContainsKey(uri))
            {
                return _cache[uri];
            }

            var doc = await _factory.StartNew(() => _web.Load(uri.ToString()));

            var response = new PageResponse(uri, doc, _web.StatusCode, _web.ResponseUri);

            _cache[uri] = response;

            return response;
        }

        public async Task<PageResponse> GetResponse(string baseUrl, string path, bool useCache = true)
        {
            return await GetResponse(BuildPath(baseUrl, path), useCache);
        }

        private Uri BuildPath(string domain, string path)
        {
            return new Uri(new Uri(domain), path);
        }
    }
}
