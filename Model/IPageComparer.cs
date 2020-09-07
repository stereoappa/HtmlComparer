namespace HtmlComparer.Model
{
    public interface IPageComparer
    {
        ICompareResult Compare(PageResponse origin, PageResponse target);
    }
}
