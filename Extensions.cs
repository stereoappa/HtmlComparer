using HtmlAgilityPack;
using HtmlComparer.Model;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HtmlComparer
{
    public static class HtmlNodesExtensions
    {
        public static IEnumerable<OutlineNode> ToOutlineNodes(this HtmlNodeCollection collection, bool exceptEmptyTags = false, bool disablePosition = true)
        {
            var prepareCollection = exceptEmptyTags ? collection.Where(x => Clear(x.InnerText) != string.Empty) :
                collection.ToList();

            for (int i = 0; i < prepareCollection.Count(); i++)
            {
                yield return new OutlineNode
                {
                    Position = disablePosition ? -1 : i,
                    TagName = prepareCollection.ElementAt(i).Name,
                    InnerText = Clear(prepareCollection.ElementAt(i).InnerText)
                };
            }
        }

        private static string Clear(string text)
        {
            return HttpUtility.HtmlDecode(text).Trim();
        }
    }

    public static class PageResponseExtensions
    {
        public static List<TagValue> FindTagValues(this PageResponse response, List<TagMetadata> compareFields)
        {
            string toLowerCase(string arg)
            {
                return $"translate(@{arg}, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')";
            }

            var attrValue = new List<TagValue>();
            foreach (var f in compareFields)
            {
                if (f.ComparedAttrName != null)
                {
                    var attr = response.ReturnedHtmlDocument.DocumentNode.SelectSingleNode($"//{f.TagName}[{toLowerCase("name")}='{f.NameAttrValue.ToLower()}']")?.Attributes[f.ComparedAttrName];

                    attrValue.Add(new TagValue
                    {
                        Path = $"{f.TagName}/{f.NameAttrValue}/{f.ComparedAttrName}",
                        Value = HttpUtility.HtmlDecode(attr?.Value ?? null)
                    });
                }
                else
                {
                    var attr = response.ReturnedHtmlDocument.DocumentNode.SelectSingleNode($"//{f.TagName}");
                    attrValue.Add(new TagValue
                    {
                        Path = attr.Name,
                        Value = HttpUtility.HtmlDecode(attr.InnerText)
                    });
                }
            }

            return attrValue;
        }
    }
}

