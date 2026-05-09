using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;
using System.Text;

namespace EchoProject.Api.Notifications.Firebase
{
    public class FirebasePushNotificationSender(
        IOptions<FirebasePushNotificationSettings> options,
        ILogger<FirebasePushNotificationSender> logger) : IFirebasePushNotificationSender
    {
        private readonly FirebasePushNotificationSettings _settings = options.Value;
        private readonly ILogger<FirebasePushNotificationSender> _logger = logger;
        private FirebaseApp? _firebaseApp;
        private readonly object _lock = new();

        public async Task<FirebasePushSendResult> SendAsync(FirebasePushNotificationRequest request, CancellationToken ct = default)
        {
            var app = GetOrCreateApp();
            if (app is null)
                return FirebasePushSendResult.Skipped;

            try
            {
                var message = new Message
                {
                    Token = request.Token,
                    Notification = new Notification
                    {
                        Title = request.Title,
                        Body = request.Body
                    },
                    Data = request.Data.ToDictionary(x => x.Key, x => x.Value)
                };

                await FirebaseMessaging.GetMessaging(app).SendAsync(message, ct);
                return FirebasePushSendResult.Sent;
            }
            catch (FirebaseMessagingException ex) when (ex.MessagingErrorCode == MessagingErrorCode.Unregistered
                || ex.MessagingErrorCode == MessagingErrorCode.InvalidArgument)
            {
                _logger.LogWarning(ex, "Firebase rejeitou o token de push {Token}.", request.Token);
                return FirebasePushSendResult.InvalidToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao enviar push notification pelo Firebase.");
                return FirebasePushSendResult.Failed;
            }
        }

        private FirebaseApp? GetOrCreateApp()
        {
            if (_firebaseApp is not null)
                return _firebaseApp;

            lock (_lock)
            {
                if (_firebaseApp is not null)
                    return _firebaseApp;

                if (!_settings.IsConfigured())
                {
                    _logger.LogInformation("Firebase Push desabilitado: configuracao ausente.");
                    return null;
                }

                try
                {
                    var json = Encoding.UTF8.GetString(Convert.FromBase64String(_settings.CredentialsBase64));
                    var credential = GoogleCredential.FromJson(json);

                    _firebaseApp = FirebaseApp.Create(new AppOptions
                    {
                        ProjectId = _settings.ProjectId,
                        Credential = credential
                    }, "echo-push");

                    return _firebaseApp;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Firebase Push desabilitado: CredentialsBase64 invalido.");
                    return null;
                }
            }
        }
    }
}
