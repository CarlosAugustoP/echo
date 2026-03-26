using EchoProject.Application.Events;
using EchoProject.Domain.DonationAggregate;
using EchoProject.Domain.Interfaces;
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

                donation.UpdateStatus(message.NewStatus);
                var statusEvent = DonationEventFactory.Create(donation, message.NewStatus);
                
                await _unitOfWork.DonationEvents.AddAsync(statusEvent);                

                await _unitOfWork.CommitAsync();

                _logger.LogInformation(">>> [Consumer] Sucesso: Banco de dados atualizado para a doação {Id}.", message.DonationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar atualização da doação {Id} no Consumer.", message.DonationId);
                throw; 
            }
        }
    }
}