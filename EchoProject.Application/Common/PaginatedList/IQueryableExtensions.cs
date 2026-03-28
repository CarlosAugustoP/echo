namespace EchoProject.Application.Common.PaginatedList
{
    public static class IQueryableExtensions
    {
        public static PaginatedList<T> Paginate<T>(this IQueryable<T> items, int page, int pageSize)
        {
            var list = items.Skip(page * pageSize).Take(pageSize);
            return new PaginatedList<T>(list, items.Count(), pageSize, page);
        }        
    }
}