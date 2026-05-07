using EchoProject.Domain.DonationAggregate;

namespace EchoProject.Domain.Notifications
{
    public interface INotification
    {
        NotificationType Type { get; }
        List<Notification> Create(INotificationModel model);
    }
}
