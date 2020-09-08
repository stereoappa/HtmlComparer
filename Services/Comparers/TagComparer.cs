using HtmlComparer.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HtmlComparer.Comparers
{
    public class TagComparer : IPagesComparer
    {
        private readonly List<ComparedTag> _compareFields;
        public TagComparer(List<ComparedTag> compareFields)
        {
            _compareFields = compareFields;
        }
        public ICompareResult Compare(PageResponse origin, PageResponse target)
        {
            var unequalFields = new List<TagValue>();
            var page1Attributes = origin.FindTagValues(_compareFields);
            var page2Attributes = target.FindTagValues(_compareFields);

            unequalFields.AddRange(page1Attributes.Except(page2Attributes, new TagValueComparer()));

            return new FieldCompareResult(origin, target, unequalFields);
        }  
    }

    public class FieldCompareResult : ICompareResult
    {
        private PageResponse _response;
        private List<TagValue> _unequalAttributes { get; }

        public FieldCompareResult(PageResponse response, PageResponse pageInfo2, List<TagValue> unequalAttrs)
        {
            _response = response;
            _unequalAttributes = unequalAttrs;
        }

        public Uri OriginPage => _response.RequestedUri;

        public bool HasErrors => _unequalAttributes.Count() > 0;

        public override string ToString()
        {
            string res = $"\tTAG COMPARER: ";
            foreach (var f in _unequalAttributes)
            {
                res += $"\r\n\tERROR: Field \"{f.Path}\" doesn't correspond on input pages\r\n";
            }
            if (!HasErrors)
            {
                res += $"OK: All fields correspond!\r\n";
            }
            return res;
        }
    }

}
