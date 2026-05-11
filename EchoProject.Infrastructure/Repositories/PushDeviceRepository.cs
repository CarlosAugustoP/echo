using EchoProject.Domain.Notifications;
using EchoProject.Domain.Repositories;
using EchoProject.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EchoProject.Infrastructure.Repositories
{
    public class PushDeviceRepository(EchoDbContext context) : EfRepository<PushDevice>(context), IPushDeviceRepository
    {
        public Task<PushDevice?> FindByTokenAsync(string token, CancellationToken ct = default)
        {
            return Query.FirstOrDefaultAsync(x => x.Token == token, ct);
        }

        public Task<List<PushDevice>> FindActiveByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            return Query
                .Where(x => x.UserId == userId && x.IsActive)
                .ToListAsync(ct);
        }
    }
}
