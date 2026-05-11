namespace EchoProject.Api.Notifications.Firebase
{
    public class FirebasePushNotificationSettings
    {
        public string? ProjectId { get; set; }
        public string? CredentialsBase64 { get; set; }

        public bool IsConfigured()
        {
            return !string.IsNullOrWhiteSpace(ProjectId)
                && !string.IsNullOrWhiteSpace(CredentialsBase64);
        }
    }
}
