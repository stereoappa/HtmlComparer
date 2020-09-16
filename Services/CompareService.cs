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

        public async Task<IEnumerable<IGrouping<string, ICompareResult>>> GetReport(Page page, bool checkRewriteRule = true, bool useCache = true)
        {
            var res = new List<ICompareResult>();

            foreach (var comparer in _comparers)
            {
                var origin = await _client.GetResponse(
                    _originSource.BaseUrl,
                    page.Path,
                    useCache);

                ThrowIfPageNotFound(origin);

                var target = await _client.GetResponse(
                    _targetSource.BaseUrl,
                    page.Path,
                    useCache);

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
                throw new WebException($"The page {resp.RequestedUri} is not available.\r\n\tStatus code: {(int)resp.StatusCode}. The page is not accepted for comparison");
            }
        }
    }
}
