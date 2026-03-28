using System.Linq.Expressions;
using EchoProject.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EchoProject.Infrastructure.Repositories
{
    public class EfRepository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _model;

        public EfRepository(DbContext context)
        {
            _context = context;
            _model = context.Set<T>();
        }
        protected virtual IQueryable<T> Query => _model;

        public async Task<T?> FindByIdAsync(Guid id, CancellationToken ct = default)
        {
            var keyName = _context.Model.FindEntityType(typeof(T))!
                .FindPrimaryKey()!
                .Properties
                .Select(x => x.Name)
                .Single();

            return await Query.FirstOrDefaultAsync(
                e => EF.Property<Guid>(e, keyName) == id,
                ct);
        }

        public async Task<T?> FindAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken ct = default)
            => await Query.FirstOrDefaultAsync(predicate, ct);

        public async Task<List<T>> ListAsync(
            Expression<Func<T, bool>>? predicate = null,
            CancellationToken ct = default)
        {
            IQueryable<T> query = Query;

            if (predicate is not null)
                query = query.Where(predicate);

            return await query.ToListAsync(ct);
        }

        public async Task AddAsync(T entity, CancellationToken ct = default)
            => await _model.AddAsync(entity, ct);

        public void Update(T entity)
            => _model.Update(entity);

        public void Remove(T entity)
            => _model.Remove(entity);

        public IQueryable<T> FindAll(Expression<Func<T, bool>>? predicate = null)
        {
            return Query;
        }
    }
}