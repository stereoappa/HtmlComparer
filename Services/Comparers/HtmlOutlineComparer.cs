using HtmlAgilityPack;
using MetaComparer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaComparer
{
    public class HtmlOutlineComparer
    { 
        private const string headers = "//*[self::h1 or self::h2 or self::h3 or self::h4]";
        public ICompareResult Compare(PageResponse origin, PageResponse target)
        {
            var originNodes = origin.FindNodesByXpath(headers);
            var targetNodes = target.FindNodesByXpath(headers);

            List<HtmlNode> badNodes = new List<HtmlNode>();
            for (int i = 0; i < originNodes.Count; i++)
            {
                var res = originNodes[i].InnerHtml == targetNodes[i].InnerHtml &&
                    originNodes[i].NodeType == targetNodes[i].NodeType;
                if(!res)
                {
                    badNodes.Add(targetNodes[i]);
                }
            }

            return new HtmlOutlineCompareResult(null, null);
        }
    }

    public class HtmlOutlineCompareResult : ICompareResult
    {
        public HtmlOutlineCompareResult(PageResponse origin, PageResponse target)
        {

        }
        public bool IsEquals { get; }

        public int MyProperty { get; set; }
    }
}
