namespace EchoProject.Domain.Notifications
{
    public static class NotificationFactory
    {
        public static INotification Create(NotificationType type)
        {
            return type switch
            {
                NotificationType.TransferConfirmed => new TransferConfirmedNotification(),
                NotificationType.SendToVendorConfirmed => new SendToVendorConfirmedNotification(),
                NotificationType.SendToNGOConfirmed => new SendToNGOConfirmedNotification(),
                _ => throw new ArgumentException("Invalid notification type")
            };
        }

        public static List<DonationAggregate.Notification> Create(NotificationType type, INotificationModel model)
        {
            return Create(type).Create(model);
        }
    }
}
