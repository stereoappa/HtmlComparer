using HtmlComparer.Model;
using System;
using System.Collections.Generic;
using System.Linq;


namespace HtmlComparer.Services.Comparers
{
    public class HtmlOutlineComparer : IPagesComparer
    {
        private const string headers = "//*[self::h1 or self::h2 or self::h3 or self::h4]";
        public IReportRow Compare(PageResponse origin, PageResponse target)
        {
            var originNodes = origin.FindNodesByXpath(headers).ToSimpleOutlineNodes(true).ToList();
            var targetNodes = target.FindNodesByXpath(headers).ToSimpleOutlineNodes().ToList();

            var badNodes = originNodes.Except(targetNodes);

            return new HtmlOutlineCompareResult(origin.RequestedUri, originNodes, targetNodes, badNodes);
        }
    }

    public class HtmlOutlineCompareResult : IReportRow
    {
        public Uri PageUri { get; }

        private List<OutlineNode> originNodes;
        private List<OutlineNode> targetNodes;
        private List<OutlineNode> badNodes;

        public HtmlOutlineCompareResult(Uri source, IEnumerable<OutlineNode> originNodes, IEnumerable<OutlineNode> targetNodes, IEnumerable<OutlineNode> badNodes)
        {
            this.originNodes = originNodes.ToList();
            this.targetNodes = targetNodes.ToList();
            this.badNodes = badNodes.ToList();
            this.PageUri = source;
        }

        public bool HasErrors => badNodes.Any(); 

        public override string ToString()
        {
            string res = $"\tHTML OUTLINE COMPARER: ";

            if (!HasErrors)
            {
                return res += "OK: Page outlines is identical\r\n";
            }

            for (int i = 0; i < badNodes.Count(); i++)
            {
                var badNode = badNodes[i];
                var originNode = originNodes[badNode.Position];
                var expectedNode = targetNodes.FirstOrDefault(x => x.Position == badNode.Position);

                res += $"\r\n\tERROR: At index {badNode.Position} expected value\r\n\t<{originNode.TagName}: {originNode.Text}>," +
                    $" but received\r\n\t<{expectedNode?.TagName ?? "EMPTY"}: {expectedNode?.Text ?? "EMPTY"}>\r\n";
            }

            return res;
        }

    }
}

