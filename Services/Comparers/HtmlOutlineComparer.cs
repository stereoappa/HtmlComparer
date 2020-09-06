using HtmlAgilityPack;
using HtmlComparer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlComparer
{
    public class HtmlOutlineComparer
    {
        private const string headers = "//*[self::h1 or self::h2 or self::h3 or self::h4]";
        public ICompareResult Compare(PageResponse origin, PageResponse target)
        {
            var originNodes = origin.FindNodesByXpath(headers).ToFlatHtmlNodes().ToList();
            var targetNodes = target.FindNodesByXpath(headers).ToFlatHtmlNodes().ToList();

            var badNodes = originNodes.Except(targetNodes);

            return new HtmlOutlineCompareResult(origin.RequestedUri, originNodes, targetNodes, badNodes);
        }
    }

    public class HtmlOutlineCompareResult : ICompareResult
    {
        private List<FlatHtmlNode> originNodes;
        private List<FlatHtmlNode> targetNodes;
        private List<FlatHtmlNode> badNodes;
        private Uri source;

        public HtmlOutlineCompareResult(Uri source, IEnumerable<FlatHtmlNode> originNodes, IEnumerable<FlatHtmlNode> targetNodes, IEnumerable<FlatHtmlNode> badNodes)
        {
            this.originNodes = originNodes.ToList();
            this.targetNodes = targetNodes.ToList();
            this.badNodes = badNodes.ToList();
            this.source = source;
        }

        public bool IsEquals => !badNodes.Any();

        public override string ToString()
        {
            string res = $"Source: {source.LocalPath}";

            if (IsEquals)
            {
                return "OK: Page outlines is identical";
            }

            for (int i = 0; i < badNodes.Count(); i++)
            {
                var badNode = badNodes[i];
                var originNode = originNodes[i];
                var expecteNode = targetNodes.FirstOrDefault(x => x.Position == badNode.Position);

                res += $"\r\nERROR: At index {badNode.Position} expected value\r\n<{originNode.Tag}: {originNode.Text}>," +
                    $" but received\r\n<{expecteNode?.Tag ?? "EMPTY"}: {expecteNode?.Text ?? "EMPTY"}>\r\n";
            }

            return res;
        }

    }
}

