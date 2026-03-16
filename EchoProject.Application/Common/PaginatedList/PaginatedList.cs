namespace EchoProject.Application.Common.PaginatedList
{
    public class PaginatedList<T>(IEnumerable<T> items, int totalCount, int pageSize, int currentPage)
    {
        public List<T> Items { get; set; } = items.ToList();
        public int TotalCount { get; set; } = totalCount;
        public int PageSize { get; set; } = pageSize;
        public int CurrentPage { get; set; } = currentPage;
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        public PaginatedList<TDestination> Select<TDestination>(Func<T, TDestination> selector)
        {
            var selectedItems = Items.Select(selector).ToList();
            return new PaginatedList<TDestination>(selectedItems, TotalCount, PageSize, CurrentPage);
        }
    }
}