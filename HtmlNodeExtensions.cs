using HtmlAgilityPack;
using HtmlComparer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlComparer
{
    public static class HtmlNodeExtensions
    {
        public static IEnumerable<FlatHtmlNode> ToFlatHtmlNodes (this HtmlNodeCollection collection)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                yield return new FlatHtmlNode
                {
                    Position = i,
                    Tag = collection[i].Name,
                    Text = collection[i].InnerText.Trim()
                };
            }
                     
        }
    }
}

