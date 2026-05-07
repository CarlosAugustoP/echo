using EchoProject.Application.Events;
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
            _logger.LogInformation(">>> [Consumer] Processando atualizacao de status: Doacao {Id} para {Status}",
                message.DonationId, message.NewStatus);

            try
            {
                var donation = await _unitOfWork.Donations.FindByIdAsync(message.DonationId);
                if (donation == null)
                {
                    _logger.LogWarning("Doacao {Id} nao encontrada no banco de dados. Ignorando.", message.DonationId);
                    return;
                }

                if (donation.Status == message.NewStatus)
                {
                    _logger.LogInformation(">>> [Consumer] A doacao {Id} ja esta com o status {Status}. Processamento ignorado.",
                        message.DonationId, message.NewStatus);
                    return;
                }

                if (message.FundsReleasedHash != null)
                {
                    _logger.LogInformation(">>> [Consumer] Atualizando hash de liberacao de fundos para a doacao {Id}.",
                        message.DonationId);
                    donation.SetFundsReleasedHash(message.FundsReleasedHash);
                }

                donation.UpdateStatus(message.NewStatus);
                var statusEvent = donation.AddEvent(message.NewStatus);
                _unitOfWork.Donations.AddDonationEvent(statusEvent);

                var notificationRequest = donation.GetNotificationRequest();
                if (notificationRequest is not null)
                {
                    foreach (var notification in NotificationFactory.Create(notificationRequest.Type, notificationRequest.Model))
                    {
                        await _unitOfWork.Notifications.AddAsync(notification);
                    }
                }

                await _unitOfWork.CommitAsync();

                _logger.LogInformation(">>> [Consumer] Sucesso: Banco de dados atualizado para a doacao {Id}.",
                    message.DonationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar atualizacao da doacao {Id} no Consumer.", message.DonationId);
                throw;
            }
        }
    }
}
