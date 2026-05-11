using EchoProject.Api.Notifications;
using EchoProject.Api.Notifications.Firebase;
using EchoProject.Application.Notifications;

namespace EchoProject.Api.DependencyInjection
{
    public static class NotificationsServiceCollectionExtensions
    {
        public static IServiceCollection AddNotificationsInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSignalR();
            services.Configure<FirebasePushNotificationSettings>(configuration.GetSection("FirebasePushNotifications"));
            services.AddSingleton<IFirebasePushNotificationSender, FirebasePushNotificationSender>();
            services.AddScoped<INotificationDeliveryService, SignalRNotificationDeliveryService>();

            return services;
        }
    }
}
