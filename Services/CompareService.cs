using HtmlComparer.Comparers;
using HtmlComparer.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HtmlComparer.Services
{
    public class CompareService
    {
        private readonly IEnumerable<IPageComparer> _comparers;
        private readonly Source _originSource;
        private readonly Source _targetSource;
        private readonly PageHttpClient _client = new PageHttpClient();

        private readonly List<Source> _sources;
        private readonly List<Page> _pages;

        public CompareService(IEnumerable<IPageComparer> comparers, List<Source> sources, List<Page> pages)
        {
            _comparers = comparers;

            _sources = sources;
            _pages = pages;
            _originSource = _sources.First(x => x.CompareRole == CompareRole.Origin);
            _targetSource = _sources.First(x => x.CompareRole == CompareRole.Target);
        }

        public async Task<List<ICompareResult>> Compare()
        {
            var res = new List<ICompareResult>();

            foreach (var comparer in _comparers)
            {
                foreach (var page in _pages)
                {
                    var origin = await _client.GetResponse(
                        _originSource.BaseUrl,
                        page.Path);
                    var target = await _client.GetResponse(
                        _targetSource.BaseUrl,
                        page.Path);

                    res.Add(comparer.Compare(origin, target));
                }
            }
            
            return res;
        }

        public async Task<List<ICompareResult>> CheckRewriteRule()
        {
            var res = new List<ICompareResult>();
            if (!_targetSource.ReturnUrlIsLowerCase)
            {
                return res;
            }

            foreach (var page in _pages)
            {
                var pi = await _client.GetResponse(_targetSource.BaseUrl, page.Path);
                res.Add(new UriRewriteChecker().Compare(pi));
            }

            return res;
        }
    }
}
