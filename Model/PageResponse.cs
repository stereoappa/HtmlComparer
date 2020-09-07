using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Web;

namespace HtmlComparer.Model
{
    public class PageResponse
    {
        public Uri ReturnedUri { get; set; }
        public Uri RequestedUri { get; set; }

        public HtmlDocument ReturnedHtmlDocument { get; set; }

        public List<TagValue> FindTagValues(List<ComparedTag> compareFields)
        {
            string toLowerCase(string arg)
            {
                return $"translate(@{arg}, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')";
            }

            var attrValue = new List<TagValue>();
            foreach (var f in compareFields)
            {
                if (f.Attr != null)
                {
                    var attr = ReturnedHtmlDocument.DocumentNode.SelectSingleNode($"//{f.Tag}[{toLowerCase("name")}='{f.Name.ToLower()}']")?.Attributes[f.Attr];

                    attrValue.Add(new TagValue
                    {
                        Path = $"{f.Tag}/{f.Name}/{f.Attr}",
                        Value = HttpUtility.HtmlDecode(attr?.Value ?? null)
                    });
                }
                else
                {
                    var attr = ReturnedHtmlDocument.DocumentNode.SelectSingleNode($"//{f.Tag}");
                    attrValue.Add(new TagValue
                    {
                        Path = attr.Name,
                        Value = HttpUtility.HtmlDecode(attr.InnerText)
                    });
                }
            }

            return attrValue;
        }

        public HtmlNodeCollection FindNodesByXpath(string xpath)
        {
            return ReturnedHtmlDocument?.DocumentNode.SelectNodes(xpath);
        }
    }

    public class TagValue
    {
        public string Path { get; set; }
        public string Value { get; set; }
    }

    public class TagValueComparer : IEqualityComparer<TagValue>
    {
        public int GetHashCode(TagValue co)
        {
            return 0;
        }

        public bool Equals(TagValue x1, TagValue x2)
        {
            return x1.Path == x2.Path &&
                 x1.Value?.Trim() == x2.Value?.Trim();
        }
    }
}
