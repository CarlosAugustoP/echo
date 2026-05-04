namespace EchoProject.Domain.Notifications
{
    public class NotificationFactory
    {
        public static INotification Create(NotificationType type)
        {
            //TODO
            return type switch
            {
                NotificationType.TransferConfirmed => new TransferConfirmedNotification(),
                NotificationType.SendToVendorConfirmed => new SendToVendorConfirmedNotification(),
                NotificationType.SendToNGOConfirmed => new SendToNGOConfirmedNotification(),
                _ => throw new ArgumentException("Invalid notification type")
            };
        }
    }
}
