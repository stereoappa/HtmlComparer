namespace HtmlComparer.Model
{
    public interface IPagesComparer
    {
        ICompareResult Compare(PageResponse origin, PageResponse target);
    }
}
