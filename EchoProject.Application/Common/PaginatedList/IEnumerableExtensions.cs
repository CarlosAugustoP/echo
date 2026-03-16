namespace EchoProject.Application.Common.PaginatedList
{
    public static class IEnumerableExtensions
    {
        public static PaginatedList<T> Paginate<T>(this IEnumerable<T> items, int page, int pageSize)
        {
            IQueryable<T> actualItems = items.AsQueryable();
            var list = actualItems.Skip(page * pageSize).Take(pageSize);
            return new PaginatedList<T>(list, actualItems.Count(), pageSize, page);
        }
    }
}