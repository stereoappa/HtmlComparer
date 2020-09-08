using HtmlAgilityPack;
using HtmlComparer.Model;
using System.Collections.Generic;

namespace HtmlComparer
{
    public static class HtmlNodesExtensions
    {
        public static IEnumerable<OutlineNode> ToSimpleOutlineNodes (this HtmlNodeCollection collection)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                yield return new OutlineNode
                {
                    Position = i,
                    Tag = collection[i].Name,
                    Text = collection[i].InnerText.Trim()
                };
            }
                     
        }
    }
}

