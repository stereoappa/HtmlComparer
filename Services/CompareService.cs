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
        private readonly IEnumerable<IPageChecker> _checkers;
        private readonly Source _originSource;
        private readonly Source _targetSource;

        public CompareService(IEnumerable<IPagesComparer> comparers, IEnumerable<IPageChecker> checkers, Source originSource, Source targetSource)
        {
            _comparers = comparers;
            _checkers = checkers;
            _originSource = originSource;
            _targetSource = targetSource;
        }

        public async Task<Report> GetReport(Page page, bool useCache = true)
        {
            var rows = new List<IReportRow>();

            var origin = await _client.GetResponse(
                    _originSource.BaseUrl,
                    page.Path,
                    useCache);
            origin.ThrowIfPageNotFound();

            var target = await _client.GetResponse(
                _targetSource.BaseUrl,
                page.Path,
                useCache);
            target.ThrowIfPageNotFound();

            foreach (var comparer in _comparers)
            {
                rows.Add(comparer.Compare(origin, target));
            }

            foreach (var checker in _checkers)
            {
                if (_originSource.UseCheckers)
                    rows.Add(checker.Check(origin));
                if (_targetSource.UseCheckers)
                    rows.Add(checker.Check(target));
            }

            return new Report(origin.RequestedUri, rows);
        }
    }
}
