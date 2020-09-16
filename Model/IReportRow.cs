using System;

namespace HtmlComparer.Model
{
    public interface IReportRow
    {
        Uri PageUri { get; }
        bool HasErrors { get; }
        string ToString();
    }
}
