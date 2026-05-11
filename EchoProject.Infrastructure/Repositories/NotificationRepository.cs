using EchoProject.Domain.DonationAggregate;
using EchoProject.Domain.Repositories;
using EchoProject.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EchoProject.Infrastructure.Repositories
{
    public class NotificationRepository(EchoDbContext context) : EfRepository<Notification>(context), INotificationRepository
    {
        protected override IQueryable<Notification> Query => base.Query.Include(x => x.SentToUser);

        public IQueryable<Notification> FindByUserId(Guid userId, CancellationToken ct = default)
        {
            return Query.Where(x => x.SentTo == userId);
        }

        public Task<int> CountUnreadByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            return Query.CountAsync(x => x.SentTo == userId && !x.IsRead, ct);
        }

        public Task<List<Notification>> FindByIdsAndUserIdAsync(IEnumerable<Guid> notificationIds, Guid userId, CancellationToken ct = default)
        {
            var ids = notificationIds.Distinct().ToList();
            return Query.Where(x => x.SentTo == userId && ids.Contains(x.Id)).ToListAsync(ct);
        }
    }
}
