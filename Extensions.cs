using HtmlAgilityPack;
using HtmlComparer.Model;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HtmlComparer
{
    public static class HtmlNodesExtensions
    {
        public static IEnumerable<OutlineNode> ToSimpleOutlineNodes(this HtmlNodeCollection collection, bool exceptEmptyTags = false)
        {
            var prepareCollection = exceptEmptyTags ? collection.Where(x => Clear(x.InnerText) != string.Empty) :
                collection.ToList();

            for (int i = 0; i < prepareCollection.Count(); i++)
            {
                yield return new OutlineNode
                {
                    Position = i,
                    Tag = prepareCollection.ElementAt(i).Name,
                    Text = Clear(prepareCollection.ElementAt(i).InnerText)
                };
            }
        }

        private static string Clear(string text)
        {
            return HttpUtility.HtmlDecode(text).Trim();
        }
    }
}

