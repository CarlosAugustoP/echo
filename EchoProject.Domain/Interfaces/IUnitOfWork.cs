using System.Data;
using EchoProject.Domain.Repositories;

namespace EchoProject.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGoalRepository Goals { get; }
        IProjectRepository Projects { get; }
        IUserRepository Users { get; }
        IDonationRepository Donations { get; }
        IGoalTypeRepository GoalTypes { get; }
        IVendorRepository Vendors { get; }

        int Commit();
        Task<int> CommitAsync(CancellationToken ct = default);

        IDbTransaction BeginTransaction();
        Task<IDbTransaction> BeginTransactionAsync(CancellationToken ct = default);

        void Rollback();
        Task RollbackAsync(CancellationToken ct = default);
    }
}