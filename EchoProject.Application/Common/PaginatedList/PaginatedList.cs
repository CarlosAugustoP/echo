namespace EchoProject.Application.Common.PaginatedList
{
    public class PaginatedList<T>
    {
        public List<T> Items { get; set; } = [];
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public PaginatedList(IEnumerable<T> items, int totalCount, int pageSize, int currentPage)
        {
            Items = items.ToList();
            TotalCount = totalCount;
            PageSize = pageSize;
            CurrentPage = currentPage;
        }
    public PaginatedList<TDestination> Select<TDestination>(Func<T, TDestination> selector)
    {
        var selectedItems = Items.Select(selector).ToList();
        return new PaginatedList<TDestination>(selectedItems, TotalCount, PageSize, CurrentPage);
    }
    }
}