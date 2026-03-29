using EchoProject.Domain.UserAggregate;

namespace EchoProject.Domain.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> FindByEmailAsync(string email, CancellationToken ct = default);
        Task<User?> FindByWalletAddressAsync(string walletAddress, CancellationToken ct = default);
        Task<User?> FindByTaxId(string taxId, CancellationToken ct = default);
        IQueryable<User> FindByRole(UserRole role, CancellationToken ct = default);
    }
}