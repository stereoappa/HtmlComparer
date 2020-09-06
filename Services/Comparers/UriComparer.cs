using HtmlComparer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlComparer
{
    public class UriComparer
    {
        public ICompareResult Compare(Model.PageResponse pageInfo)
        {
            var res = pageInfo.RequestedUri.LocalPath.ToLower() == pageInfo.ReturnedUri.LocalPath;

            return new UriCompareResult(pageInfo, res);
        }
    }

    public class UriCompareResult : ICompareResult
    {
        public Model.PageResponse Page { get;  }

        public bool IsEquals { get; }
        public UriCompareResult(Model.PageResponse page, bool uriIsEquals)
        {
            Page = page;
            IsEquals = uriIsEquals;
        }

        public override string ToString()
        {
            string res = $"Source: {Page.RequestedUri.LocalPath}:\r\n";
            if(!IsEquals)
            {
                res += $"------- ERROR: Return Uri is incorrect {Page.RequestedUri.LocalPath}\r\n";
                return res;  
            } 
            else
            {
                res += "------- OK: Return Url is lower case\r\n";
                return res;
            }

           
        }
    }
}
