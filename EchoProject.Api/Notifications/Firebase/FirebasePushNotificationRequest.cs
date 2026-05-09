namespace EchoProject.Api.Notifications.Firebase
{
    public record FirebasePushNotificationRequest(
        string Token,
        string Title,
        string Body,
        IReadOnlyDictionary<string, string> Data);
}
