namespace EchoProject.Application.Requests.Notifications
{
    public record MarkNotificationsAsReadRequest(List<Guid> NotificationIds);
}
