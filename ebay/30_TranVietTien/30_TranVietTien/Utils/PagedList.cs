public class PagedList<T>
{
    public IReadOnlyList<T> Items { get; }
    public int Page { get; }
    public int PageSize { get; }
    public int TotalItems { get; }
    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);

    public PagedList(IEnumerable<T> items, int page, int pageSize, int total)
    {
        Items = items.ToList();
        Page = page; PageSize = pageSize; TotalItems = total;
    }
}
