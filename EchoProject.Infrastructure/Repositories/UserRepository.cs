using EchoProject.Domain.Repositories;
using EchoProject.Domain.UserAggregate;
using EchoProject.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EchoProject.Infrastructure.Repositories
{
    public class UserRepository(EchoDbContext context) : EfRepository<User>(context), IUserRepository
    {
        public async Task<User?> FindByEmailAsync(string email, CancellationToken ct = default)
        {
            return await Query.FirstOrDefaultAsync(u => u.Email == email, ct);
        }

        public async Task<User?> FindByTaxId(string taxId, CancellationToken ct = default)
        {
            return await Query.FirstOrDefaultAsync(u => u.TaxId.Value == taxId, ct);
        }

        public async Task<User?> FindByWalletAddressAsync(string walletAddress, CancellationToken ct = default)
        {
            return await Query.FirstOrDefaultAsync(u => u.WalletAddress.Address == walletAddress, ct);
        }

        public IQueryable<User> FindByRole(UserRole role, CancellationToken ct = default)
        {
            return Query.Where(u => u.Role == role);
        }

        public IQueryable<User> SearchNgos(string? search, CancellationToken ct = default)
        {
            var ngos = Query.Where(u => u.Role == UserRole.NGO);

            if (string.IsNullOrWhiteSpace(search))
                return ngos.OrderBy(u => u.Name);

            var term = search.Trim().ToLower();
            return ngos
                .Where(u =>
                    u.Name.ToLower().Contains(term) ||
                    (u.Bio ?? string.Empty).ToLower().Contains(term))
                .OrderBy(u => u.Name);
        }
    }
}
