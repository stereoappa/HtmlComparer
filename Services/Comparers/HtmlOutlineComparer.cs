using HtmlComparer.Model;
using System;
using System.Collections.Generic;
using System.Linq;


namespace HtmlComparer.Services.Comparers
{
    public class HtmlOutlineComparer : IPagesComparer
    {
        private const string headers = "//*[self::h1 or self::h2 or self::h3 or self::h4]";
        IEnumerable<Page> _disabledOutlinePositionPages;

        public HtmlOutlineComparer(IEnumerable<Page> disabledOutlinePositionPages = null)
        {
            _disabledOutlinePositionPages = disabledOutlinePositionPages;
        }

       
        public IReportRow Compare(PageResponse origin, PageResponse target)
        {
            var disablePosition = IsDisabledOutlinePosition(origin);

            var originNodes = origin.FindNodesByXpath(headers).ToOutlineNodes(exceptEmptyTags: true, disablePosition).ToList();
            var targetNodes = target.FindNodesByXpath(headers).ToOutlineNodes(disablePosition: disablePosition).ToList();

            var badNodes = originNodes.Except(targetNodes);

            return new HtmlOutlineCompareResult(origin.RequestedUri, originNodes, targetNodes, badNodes, disablePosition);
        }

        private bool IsDisabledOutlinePosition(PageResponse origin)
        {
            if (_disabledOutlinePositionPages == null)
                return false;

            return _disabledOutlinePositionPages
                .Any(x => origin.RequestedUri.LocalPath.ToLower() == x.LocalPath.ToLower());
        }

        class HtmlOutlineCompareResult : IReportRow
        {
            public Uri PageUri { get; }

            private List<OutlineNode> originNodes;
            private List<OutlineNode> targetNodes;
            private List<OutlineNode> badNodes;
            private bool _isDisablePositionCheck;

            public HtmlOutlineCompareResult(Uri source, 
                IEnumerable<OutlineNode> originNodes, 
                IEnumerable<OutlineNode> targetNodes, 
                IEnumerable<OutlineNode> badNodes,
                bool disablePositioning)
            {
                this.originNodes = originNodes.ToList();
                this.targetNodes = targetNodes.ToList();
                this.badNodes = badNodes.ToList();
                this.PageUri = source;
                this._isDisablePositionCheck = disablePositioning;
            }

            public bool HasErrors => badNodes.Any();

            public override string ToString()
            {
                string res = $"\tHTML OUTLINE COMPARER" + (_isDisablePositionCheck ? " (tag positions weren't compared)" : "") + ": ";

                if (!HasErrors)
                {
                    return res += "OK: Page outlines are identical\r\n";
                }          

                for (int i = 0; i < badNodes.Count(); i++)
                {
                    var badNode = badNodes[i];
                    if (_isDisablePositionCheck)
                    {
                        res += $"\r\n\tERROR: Value <{badNode.TagName}: {badNode.InnerText ?? "EMPTY"}> wasn't found in the target outline\r\n";
                        continue;
                    }

                    var originNode = originNodes[badNode.Position];
                    var expectedNode = targetNodes.FirstOrDefault(x => x.Position == badNode.Position);

                    res += $"\r\n\tERROR: At index {badNode.Position} expected value\r\n\t<{originNode.TagName}: {originNode.InnerText}>," +
                        $" but received\r\n\t<{expectedNode?.TagName ?? "EMPTY"}: {expectedNode?.InnerText ?? "EMPTY"}>\r\n";
                }

                return res;
            }
        }
    }
}
