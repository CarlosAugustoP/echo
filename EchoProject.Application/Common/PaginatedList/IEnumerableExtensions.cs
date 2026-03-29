namespace EchoProject.Application.Common.PaginatedList
{
    public static class IEnumerableExtensions
    {
        public static PaginatedList<T> Paginate<T>(this IEnumerable<T> items, int page, int pageSize)
        {
            var list = items.Skip(page * pageSize).Take(pageSize);
            return new PaginatedList<T>(list, items.Count(), pageSize, page);
        }      
    }
}