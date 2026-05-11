using EchoProject.Domain.DonationAggregate;

namespace EchoProject.Domain.Notifications
{
    public abstract class NotificationBase<TModel> : INotification
        where TModel : INotificationModel
    {
        public abstract NotificationType Type { get; }

        public List<Notification> Create(INotificationModel model)
        {
            if (model is not TModel typedModel)
                throw new ArgumentException($"Invalid model for {GetType().Name}");

            return CreateCore(typedModel);
        }

        protected abstract List<Notification> CreateCore(TModel model);
    }
}
