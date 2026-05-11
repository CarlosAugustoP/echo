using EchoProject.Application.Common;
using EchoProject.Application.Notifications;
using EchoProject.Domain.DonationAggregate;
using EchoProject.Domain.Interfaces;
using EchoProject.Domain.Notifications;

namespace EchoProject.Application.Services
{
    [AppService]
    public class NotificationPipelineService(IUnitOfWork unitOfWork, INotificationDeliveryService deliveryService)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly INotificationDeliveryService _deliveryService = deliveryService;

        public async Task<List<Notification>> QueueAsync(NotificationRequest? request, CancellationToken ct = default)
        {
            if (request is null)
                return [];

            var notifications = NotificationFactory.Create(request.Type, request.Model);

            foreach (var notification in notifications)
            {
                await _unitOfWork.Notifications.AddAsync(notification, ct);
            }

            return notifications;
        }

        public Task DeliverAsync(IEnumerable<Notification> notifications, CancellationToken ct = default)
        {
            return _deliveryService.DeliverAsync(notifications, ct);
        }
    }
}
