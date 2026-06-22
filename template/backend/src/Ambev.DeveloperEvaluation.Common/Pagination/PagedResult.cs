namespace Ambev.DeveloperEvaluation.Common.Pagination;

public class PagedResult<T>
{
    public IReadOnlyCollection<T> Items { get; }
    public int CurrentPage { get; }
    public int PageSize { get; }
    public int TotalPages { get; }
    public int TotalCount { get; }

    public PagedResult(IReadOnlyCollection<T> items, int totalCount, int currentPage, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        CurrentPage = currentPage;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }
}
