using EchoProject.Domain.DonationAggregate;

namespace EchoProject.Application.Notifications
{
    public interface INotificationDeliveryService
    {
        Task DeliverAsync(IEnumerable<Notification> notifications, CancellationToken ct = default);
    }
}
