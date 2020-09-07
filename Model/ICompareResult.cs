using System;

namespace HtmlComparer.Model
{
    public interface ICompareResult
    {
        Uri OriginPage { get; }
        bool HasErrors { get; }
    }
}
