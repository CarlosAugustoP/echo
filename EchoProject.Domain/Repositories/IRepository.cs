using System.Linq.Expressions;

namespace EchoProject.Domain.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T?> FindByIdAsync(Guid id, CancellationToken ct = default);
        Task<T?> FindAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken ct = default);
        Task<List<T>> ListAsync(
            Expression<Func<T, bool>>? predicate = null,
            CancellationToken ct = default);

        IQueryable<T> FindAll(Expression<Func<T, bool>>? predicate = null);
        
        Task AddAsync(T entity, CancellationToken ct = default);
        void Update(T entity);
        void Remove(T entity);
    }
}