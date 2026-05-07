namespace EchoProject.Domain.Notifications
{
    public record NotificationRequest(NotificationType Type, INotificationModel Model);
}
