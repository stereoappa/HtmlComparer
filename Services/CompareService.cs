using HtmlComparer.Comparers;
using HtmlComparer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace HtmlComparer.Services
{
    public class CompareService
    {
        private readonly PageHttpClient _client = new PageHttpClient();

        private readonly IEnumerable<IPagesComparer> _comparers;
        private readonly Source _originSource;
        private readonly Source _targetSource;

        public CompareService(IEnumerable<IPagesComparer> comparers, Source originSource, Source targetSource)
        {
            _comparers = comparers;
            _originSource = originSource;
            _targetSource = targetSource;
        }

        public async Task<IEnumerable<IGrouping<string, ICompareResult>>> GetCompareReport(Page page, bool checkRewriteRule = true)
        {
            var res = new List<ICompareResult>();

            foreach (var comparer in _comparers)
            {
                var origin = await _client.GetResponse(
                    _originSource.BaseUrl,
                    page.Path);

                ThrowIfPageNotFound(origin);

                var target = await _client.GetResponse(
                    _targetSource.BaseUrl,
                    page.Path);

                ThrowIfPageNotFound(target); 

                res.Add(comparer.Compare(origin, target));
            }

            if (checkRewriteRule)
            {
                var withRewriteRuleResults = await CheckRewriteRule(page);
                res.AddRange(withRewriteRuleResults);
            }           
            
            return res.GroupBy(x => x.OriginPage.LocalPath.ToLower());
        }

        public async Task<IEnumerable<ICompareResult>> CheckRewriteRule(Page page)
        {
            var res = new List<ICompareResult>();
            if (!_targetSource.ReturnUrlIsLowerCase)
            {
                return res;
            }

            var pi = await _client.GetResponse(_targetSource.BaseUrl, page.Path);
            res.Add(new UriRewriteChecker().Compare(pi));

            return res;
        }
        
        private void ThrowIfPageNotFound(PageResponse resp)
        {
            if (resp.StatusCode != HttpStatusCode.OK)
            {
                throw new WebException($"Source: {resp.RequestedUri}\r\n\tHTTP ERROR: Status code: {(int)resp.StatusCode}. The page is not accepted for comparison");
            }
        }
    }
}
