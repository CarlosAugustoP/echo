using EchoProject.Application.Events;
using EchoProject.Application.Services;
using EchoProject.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;

namespace EchoProject.Application.Consumers
{
    public class DonationStatusUpdatedConsumer : IHandleMessages<DonationStatusUpdatedMessage>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DonationStatusUpdatedConsumer> _logger;
        private readonly NotificationPipelineService _notificationPipeline;

        public DonationStatusUpdatedConsumer(
            IUnitOfWork unitOfWork,
            ILogger<DonationStatusUpdatedConsumer> logger,
            NotificationPipelineService notificationPipeline)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _notificationPipeline = notificationPipeline;
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

                var notifications = await _notificationPipeline.QueueAsync(donation.GetNotificationRequest());

                await _unitOfWork.CommitAsync();
                await _notificationPipeline.DeliverAsync(notifications);

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
