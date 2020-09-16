using HtmlComparer.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HtmlComparer.Services.Comparers
{
    public class TagComparer : IPagesComparer
    {
        private readonly List<TagMetadata> _compareFields;
        public TagComparer(List<TagMetadata> compareFields)
        {
            _compareFields = compareFields;
        }
        public IReportRow Compare(PageResponse origin, PageResponse target)
        {
            var unequalFields = new List<TagValue>();
            var page1Attributes = origin.FindTagValues(_compareFields);
            var page2Attributes = target.FindTagValues(_compareFields);

            unequalFields.AddRange(page1Attributes.Except(page2Attributes, new TagValueComparer()));

            return new TagCompareResult(origin, unequalFields);
        }

        class TagValueComparer : IEqualityComparer<TagValue>
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

    public class TagCompareResult : IReportRow
    {
        private PageResponse _response;
        private List<TagValue> _unequalAttributes { get; }

        public TagCompareResult(PageResponse origin, List<TagValue> unequalAttrs)
        {
            _response = origin;
            _unequalAttributes = unequalAttrs;
        }

        public Uri PageUri => _response.RequestedUri;

        public bool HasErrors => _unequalAttributes.Count() > 0;

        public override string ToString()
        {
            string res = $"\tTAG COMPARER: ";
            foreach (var f in _unequalAttributes)
            {
                res += $"\r\n\tERROR: Tag \"{f.Path}\" doesn't correspond on input pages\r\n";
            }
            if (!HasErrors)
            {
                res += $"OK: All tags correspond!\r\n";
            }
            return res;
        }
    }  
}
