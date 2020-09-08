using HtmlAgilityPack;
using HtmlComparer.Model;
using System.Collections.Generic;
using System.Linq;

namespace HtmlComparer
{
    public static class HtmlNodesExtensions
    {
        public static IEnumerable<OutlineNode> ToSimpleOutlineNodes(this HtmlNodeCollection collection, bool exceptEmptyTags = false)
        {
            var prepareCollection = exceptEmptyTags ? collection.Where(x => x.InnerText.Trim() != string.Empty) :
                collection.ToList();

            for (int i = 0; i < prepareCollection.Count(); i++)
            {
                yield return new OutlineNode
                {
                    Position = i,
                    Tag = prepareCollection.ElementAt(i).Name,
                    Text = prepareCollection.ElementAt(i).InnerText.Trim()
                };


            }

        }
    }
}

