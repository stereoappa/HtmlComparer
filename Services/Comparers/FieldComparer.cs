using HtmlAgilityPack;
using HtmlComparer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlComparer
{
    public class FieldComparer
    {
       
        public ICompareResult Compare(Model.PageResponse origin, Model.PageResponse target, List<ComparedField> compareFields)
        {
            var unequalFields = new List<TagValue>();
            var page1Attributes = origin.FindTagValues(compareFields);
            var page2Attributes = target.FindTagValues(compareFields);

            unequalFields.AddRange(page1Attributes.Except(page2Attributes, new TagValueComparer()));

            return new FieldCompareResult(origin, target, unequalFields);
        }  
    }

    public class FieldCompareResult : ICompareResult
    {
        public FieldCompareResult(Model.PageResponse pageInfo1, Model.PageResponse pageInfo2, List<TagValue> unequalAttrs)
        {
            Page1 = pageInfo1;
            Page2 = pageInfo2;
            UnequalAttributes = unequalAttrs;
        }

        public List<TagValue> UnequalAttributes { get; }
        public Model.PageResponse Page1 { get; }
        public Model.PageResponse Page2 { get; }
        public bool IsEquals => UnequalAttributes.Count() == 0;

        public override string ToString()
        {
            string res = $"Source: {Page1.ReturnedUri.LocalPath}:\r\n";
            foreach (var f in UnequalAttributes)
            {
                res += $"------- ERROR: Field \"{f.Path}\" doesn't correspond on input pages\r\n";
            }
            if (IsEquals)
            {
                res += $"------- OK: All fields correspond!\r\n";
            }
            return res;
        }
    }

}
