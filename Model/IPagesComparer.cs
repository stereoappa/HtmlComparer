namespace HtmlComparer.Model
{
    public interface IPagesComparer
    {
        IReportRow Compare(PageResponse origin, PageResponse target);
    }
}
