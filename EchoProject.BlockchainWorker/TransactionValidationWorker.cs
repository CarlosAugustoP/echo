using EchoProject.Domain.DonationAggregate;
using EchoProject.Domain.Interfaces;
using EchoProject.Infrastructure.Blockchain.Interfaces;
using MassTransit; // Importante!

namespace EchoProject.BlockchainWorker
{
    public class TransactionValidationWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TransactionValidationWorker> _logger;

        public TransactionValidationWorker(IServiceProvider serviceProvider, ILogger<TransactionValidationWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker de Validação de Transações iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var ethService = scope.ServiceProvider.GetRequiredService<IEthereumService>();
                    var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                    var pendingDonations = unitOfWork.Donations.FindPendingConfirmations(stoppingToken);

                    foreach (var donation in pendingDonations)
                    {
                        _logger.LogInformation("Validando transação {Hash} para a doação {Id}", donation.TransactionHash, donation.Id);

                        var currentStatus = await ethService.GetDonationStatus(
                            donation.TransactionHash, 
                            donation.TransferredToVendor!.Wallet, 
                            donation.Amount
                        );

                        if (currentStatus != DonationStatus.TransferredToVendorPending)
                        {
                            donation.UpdateStatus(currentStatus); 
                            
                            var statusEvent = DonationEventFactory.Create(donation, currentStatus);
                            donation.AddEvent(statusEvent);

                            await publishEndpoint.Publish(new DonationStatusUpdatedMessage(
                                donation.Id, 
                                currentStatus, 
                                donation.TransactionHash
                            ), stoppingToken);

                            _logger.LogInformation("Notificação enviada ao RabbitMQ: Doação {Id} -> {Status}", donation.Id, currentStatus);
                        }
                    }

                    await unitOfWork.CommitAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}