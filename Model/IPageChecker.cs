using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlComparer.Model
{
    public interface IPageChecker
    {
        IReportRow Check(PageResponse response);
    }
}
