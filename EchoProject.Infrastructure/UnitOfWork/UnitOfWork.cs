using System.Data;
using EchoProject.Domain.Interfaces;
using EchoProject.Domain.Repositories;
using EchoProject.Infrastructure.Database;
using Microsoft.EntityFrameworkCore.Storage;

namespace EchoProject.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EchoDbContext _context;
        private IDbContextTransaction? _objTrans;
        public IGoalRepository Goals { get; }
        public IProjectRepository Projects { get; }
        public IUserRepository Users { get; }
        public IDonationRepository Donations { get; }
        public IGoalTypeRepository GoalTypes { get; }
        public IVendorRepository Vendors { get; }

        public UnitOfWork(
            EchoDbContext context,
            IGoalRepository goals,
            IProjectRepository projects,
            IUserRepository users,
            IDonationRepository donations,
            IGoalTypeRepository goalTypes,
            IVendorRepository vendors)
        {
            _context = context;
            Goals = goals;
            Projects = projects;
            Users = users;
            Donations = donations;
            GoalTypes = goalTypes;
            Vendors = vendors;
        }

        public int Commit() => _context.SaveChanges();

        public async Task<int> CommitAsync(CancellationToken ct = default)
            => await _context.SaveChangesAsync(ct);

        public IDbTransaction BeginTransaction()
        {
            _objTrans = _context.Database.BeginTransaction();
            return _objTrans.GetDbTransaction();
        }

        public async Task<IDbTransaction> BeginTransactionAsync(CancellationToken ct = default)
        {
            _objTrans = await _context.Database.BeginTransactionAsync(ct);
            return _objTrans.GetDbTransaction();
        }

        public void Rollback() => _objTrans?.Rollback();

        public async Task RollbackAsync(CancellationToken ct = default)
        {
            if (_objTrans != null) await _objTrans.RollbackAsync(ct);
        }

        public void Dispose() => _context.Dispose();
    }
}
