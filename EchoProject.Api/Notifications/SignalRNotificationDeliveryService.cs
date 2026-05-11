using AutoMapper;
using EchoProject.Api.Hubs;
using EchoProject.Api.Notifications.Firebase;
using EchoProject.Application.DTO.Notifications;
using EchoProject.Application.Notifications;
using EchoProject.Domain.DonationAggregate;
using EchoProject.Domain.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace EchoProject.Api.Notifications
{
    public class SignalRNotificationDeliveryService(
        IHubContext<NotificationHub, INotificationClient> hubContext,
        IUnitOfWork unitOfWork,
        IFirebasePushNotificationSender pushSender,
        IMapper mapper,
        ILogger<SignalRNotificationDeliveryService> logger) : INotificationDeliveryService
    {
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext = hubContext;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IFirebasePushNotificationSender _pushSender = pushSender;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<SignalRNotificationDeliveryService> _logger = logger;

        public async Task DeliverAsync(IEnumerable<Notification> notifications, CancellationToken ct = default)
        {
            var list = notifications.ToList();
            if (list.Count == 0)
                return;

            var changedDevices = false;

            foreach (var notification in list)
            {
                var dto = _mapper.Map<NotificationDTO>(notification);
                var unreadCount = await _unitOfWork.Notifications.CountUnreadByUserIdAsync(notification.SentTo, ct);
                
                _logger.LogInformation("[SIGNALR] Enviando notificação {NotificationId} para o usuário {UserId}. Unread count: {UnreadCount}.", notification.Id, notification.SentTo, unreadCount);
                
                await _hubContext.Clients.User(notification.SentTo.ToString()).NotificationReceived(dto);
                await _hubContext.Clients.User(notification.SentTo.ToString()).UnreadCountUpdated(
                    new UnreadNotificationsCountDTO { Count = unreadCount });

                var devices = await _unitOfWork.PushDevices.FindActiveByUserIdAsync(notification.SentTo, ct);
                foreach (var device in devices)
                {
                    var delivered = await _pushSender.SendAsync(new FirebasePushNotificationRequest(
                        device.Token,
                        notification.Message,
                        notification.Description,
                        new Dictionary<string, string>
                        {
                            ["notificationId"] = notification.Id.ToString(),
                            ["sentTo"] = notification.SentTo.ToString(),
                            ["createdAt"] = notification.CreatedAt.ToUniversalTime().ToString("O")
                        }), ct);

                    if (delivered == FirebasePushSendResult.Sent)
                    {
                        device.MarkAsUsed();
                        changedDevices = true;
                        continue;
                    }

                    if (delivered == FirebasePushSendResult.InvalidToken)
                    {
                        device.Deactivate();
                        changedDevices = true;
                    }
                }
            }

            if (changedDevices)
            {
                await _unitOfWork.CommitAsync(ct);
            }

            _logger.LogInformation("Entrega de notificacoes concluida para {Count} notificacao(oes).", list.Count);
        }
    }
}
