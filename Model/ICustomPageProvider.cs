using HtmlComparer.Model;
using System.Collections.Generic;

namespace HtmlComparer.Model
{
    public interface ICustomPageProvider
    {
        IEnumerable<Page> GetPages();
    }
}
