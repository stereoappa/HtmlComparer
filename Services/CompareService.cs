using HtmlComparer.Comparers;
using HtmlComparer.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HtmlComparer.Services
{
    public class CompareService
    {
        private readonly PageHttpClient _client = new PageHttpClient();

        private readonly IEnumerable<IPagesComparer> _comparers;
        private readonly IEnumerable<Page> _pages;
        private readonly Source _originSource;
        private readonly Source _targetSource;

        public CompareService(IEnumerable<IPagesComparer> comparers, Source originSource, Source targetSource)
        {
            _comparers = comparers;
            //_pages = pages;
            _originSource = originSource;
            _targetSource = targetSource;
        }

        public async Task<List<ICompareResult>> Compare(Page page)
        {
            var res = new List<ICompareResult>();

            foreach (var comparer in _comparers)
            {
                //foreach (var page in _pages)
                //{
                    var origin = await _client.GetResponse(
                        _originSource.BaseUrl,
                        page.Path);
                    var target = await _client.GetResponse(
                        _targetSource.BaseUrl,
                        page.Path);

                    res.Add(comparer.Compare(origin, target));
                //}
            }
            
            return res;
        }

        public async Task<List<ICompareResult>> CheckRewriteRule(Page page)
        {
            var res = new List<ICompareResult>();
            if (!_targetSource.ReturnUrlIsLowerCase)
            {
                return res;
            }

           // foreach (var page in _pages)
            //{
                var pi = await _client.GetResponse(_targetSource.BaseUrl, page.Path);
                res.Add(new UriRewriteChecker().Compare(pi));
            //}

            return res;
        }
    }
}
