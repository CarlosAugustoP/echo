using EchoProject.Application.Events;
using EchoProject.Domain.DonationAggregate;
using EchoProject.Domain.Interfaces;
using EchoProject.Domain.Notifications;
using Microsoft.Extensions.Logging;
using Rebus.Handlers; 

namespace EchoProject.Application.Consumers
{
    public class DonationStatusUpdatedConsumer : IHandleMessages<DonationStatusUpdatedMessage>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DonationStatusUpdatedConsumer> _logger;

        public DonationStatusUpdatedConsumer(IUnitOfWork unitOfWork, ILogger<DonationStatusUpdatedConsumer> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(DonationStatusUpdatedMessage message)
        {
            _logger.LogInformation(">>> [Consumer] Processando atualização de status: Doação {Id} para {Status}", 
                message.DonationId, message.NewStatus);

            try
            {
                var donation = await _unitOfWork.Donations.FindByIdAsync(message.DonationId);
                if (donation == null)
                {
                    _logger.LogWarning("Doação {Id} não encontrada no banco de dados. Ignorando.", message.DonationId);
                    return;
                }

                if (donation.Status == message.NewStatus)
                {
                    _logger.LogInformation(">>> [Consumer] A doação {Id} já está com o status {Status}. Processamento ignorado.", message.DonationId, message.NewStatus);
                    return;
                }

                if (message.FundsReleasedHash != null)
                {
                    _logger.LogInformation(">>> [Consumer] Atualizando hash de liberação de fundos para a doação {Id}.", message.DonationId);
                    donation.SetFundsReleasedHash(message.FundsReleasedHash);    
                }

                donation.UpdateStatus(message.NewStatus);
                var statusEvent = donation.AddEvent(message.NewStatus);
                _unitOfWork.Donations.AddDonationEvent(statusEvent);

                foreach (var notification in BuildNotifications(donation, message.NewStatus))
                {
                    await _unitOfWork.Notifications.AddAsync(notification);
                }

                await _unitOfWork.CommitAsync();

                _logger.LogInformation(">>> [Consumer] Sucesso: Banco de dados atualizado para a doação {Id}.", message.DonationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar atualização da doação {Id} no Consumer.", message.DonationId);
                throw; 
            }
        }

        private static List<Notification> BuildNotifications(Donation donation, DonationStatus newStatus)
        {
            return newStatus switch
            {
                DonationStatus.TransferredToVendorConfirmed => CreateVendorConfirmedNotifications(donation),
                DonationStatus.ImmediateTransferToNGOConfirmed => CreateNgoConfirmedNotifications(donation),
                _ => []
            };
        }

        private static List<Notification> CreateVendorConfirmedNotifications(Donation donation)
        {
            var notification = NotificationFactory.Create(NotificationType.SendToVendorConfirmed);
            notification.Store(new SendToVendorConfirmedNotification.SendToVendorConfirmedNotificationModel(
                donation.DonorId,
                donation.Goal.Project.ManagerId,
                donation.Amount,
                donation.Goal.Project.Title,
                donation.Goal.Title,
                donation.TransferredToVendor?.Name ?? "fornecedor vinculado"));

            return notification.GetNotifications();
        }

        private static List<Notification> CreateNgoConfirmedNotifications(Donation donation)
        {
            var notification = NotificationFactory.Create(NotificationType.SendToNGOConfirmed);
            notification.Store(new SendToNGOConfirmedNotification.SendToNGOConfirmedNotificationModel(
                donation.DonorId,
                donation.Goal.Project.ManagerId,
                donation.Amount,
                donation.Goal.Project.Title));

            return notification.GetNotifications();
        }
    }
}
