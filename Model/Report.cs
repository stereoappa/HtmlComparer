using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlComparer.Model
{
    public class Report
    {
        public Report(Uri uri, IEnumerable<IReportRow> results)
        {
            Uri = uri;
            Results = results;
            HasErrors = results.Any(r => r.HasErrors);
        }
        public Uri Uri { get;}
        public IEnumerable<IReportRow> Results { get; }
        public bool HasErrors { get; } 
    }
}
