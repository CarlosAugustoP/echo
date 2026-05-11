using EchoProject.Application.DTO.Notifications;

namespace EchoProject.Api.Hubs
{
    public interface INotificationClient
    {
        Task NotificationReceived(NotificationDTO notification);
        Task UnreadCountUpdated(UnreadNotificationsCountDTO payload);
    }
}
