using EchoProject.Domain.DonationAggregate;

namespace EchoProject.Domain.Notifications
{
    public interface INotification
    {
        void Store(object model);
        List<Notification> GetNotifications();
    }
}