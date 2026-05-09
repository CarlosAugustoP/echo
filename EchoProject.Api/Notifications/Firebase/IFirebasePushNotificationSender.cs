namespace EchoProject.Api.Notifications.Firebase
{
    public interface IFirebasePushNotificationSender
    {
        Task<FirebasePushSendResult> SendAsync(FirebasePushNotificationRequest request, CancellationToken ct = default);
    }
}
