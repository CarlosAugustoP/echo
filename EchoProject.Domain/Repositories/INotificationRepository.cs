using EchoProject.Domain.DonationAggregate;

namespace EchoProject.Domain.Repositories
{
    public interface INotificationRepository : IRepository<Notification>
    {
        IQueryable<Notification> FindByUserId(Guid userId, CancellationToken ct = default);
        Task<int> CountUnreadByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<List<Notification>> FindByIdsAndUserIdAsync(IEnumerable<Guid> notificationIds, Guid userId, CancellationToken ct = default);
    }
}
