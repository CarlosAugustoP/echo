using EchoProject.Application.Common;
using EchoProject.Application.Notifications;
using EchoProject.Domain.DonationAggregate;
using EchoProject.Domain.Interfaces;
using EchoProject.Domain.Notifications;
using Microsoft.Extensions.Logging;

namespace EchoProject.Application.Services
{
    [AppService]
    public class NotificationPipelineService(IUnitOfWork unitOfWork, INotificationDeliveryService deliveryService, ILogger<NotificationPipelineService> logger)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly INotificationDeliveryService _deliveryService = deliveryService;
        private readonly ILogger<NotificationPipelineService> _logger = logger;

        public async Task<List<Notification>> QueueAsync(NotificationRequest? request, CancellationToken ct = default)
        {
            _logger.LogInformation(">>> [NotificationPipeline] Recebendo solicitação de notificação do tipo {Type}.", request?.Type.ToString() ?? "null");
            
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
