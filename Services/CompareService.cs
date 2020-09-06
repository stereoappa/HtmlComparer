using HtmlComparer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlComparer.Services
{
    public class CompareService
    {
        private readonly Source _originSource;
        private readonly Source _targetSource;
        private readonly PageHttpClient _client = new PageHttpClient();

        private readonly FieldComparer _fieldComparer;
        private readonly UriComparer _uriComparer;
        private readonly HtmlOutlineComparer _htmlOutlineComparer;

        public CompareService(List<Source> sources)
        {
            _fieldComparer = new FieldComparer();
            _uriComparer = new UriComparer();
            _htmlOutlineComparer = new HtmlOutlineComparer();

            _originSource = sources.First(x => x.CompareRole == CompareRole.Origin);
            _targetSource = sources.First(x => x.CompareRole == CompareRole.Target);
        }

        public async Task<List<ICompareResult>> CompareByFields(List<Page> pages, List<ComparedField> compareFields)
        {
            var res = new List<ICompareResult>();
            foreach (var page in pages)
            {
                var origin = await _client.GetResponse(
                    _originSource.BaseUrl,
                    page.Path);
                var target = await _client.GetResponse(
                    _targetSource.BaseUrl,
                    page.Path);

                res.Add(_fieldComparer.Compare(origin, target, compareFields));
            }

            return res;
        }

        public async Task<List<ICompareResult>> CompareByHtmlOutline(List<Page> pages)
        {
            var res = new List<ICompareResult>();
            foreach (var page in pages)
            {
                var origin = await _client.GetResponse(
                    _originSource.BaseUrl,
                    page.Path);
                var target = await _client.GetResponse(
                    _targetSource.BaseUrl,
                    page.Path);

                res.Add(_htmlOutlineComparer.Compare(origin, target));
            }

            return res;
        }

        public async Task<List<ICompareResult>> CheckReturnUrl(List<Page> _pages)
        {
            var res = new List<ICompareResult>();
            if (!_targetSource.ReturnUrlIsLowerCase)
            {
                return res;
            }

            foreach (var page in _pages)
            {
                var pi = await _client.GetResponse(_targetSource.BaseUrl, page.Path);
                res.Add(_uriComparer.Compare(pi));
            }

            return res;
        }
    }
}
