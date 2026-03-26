using EchoProject.Application.Events;
using EchoProject.Domain.DonationAggregate;
using EchoProject.Domain.Interfaces;
using EchoProject.Domain.ValueObjects;
using EchoProject.Infrastructure.Blockchain.Interfaces;
using Rebus.Bus; 

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
                    
                    // Rebus: Pega a instância do IBus ao invés do IPublishEndpoint
                    var bus = scope.ServiceProvider.GetRequiredService<IBus>();

                    var pendingVendor = unitOfWork.Donations.FindPendingConfirmations(stoppingToken);
                    var pendingNGO = unitOfWork.Donations.FindDirectPendingNGOLiberation(stoppingToken);

                    await ProcessDonationsAsync(pendingVendor, DonationStatus.TransferredToVendorPending, ethService, bus, stoppingToken);
                    await ProcessDonationsAsync(pendingNGO, DonationStatus.ImmediateTransferToNGOPending, ethService, bus, stoppingToken);
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        private async Task ProcessDonationsAsync(
            IEnumerable<Donation> donations, 
            DonationStatus pendingStatus,
            IEthereumService ethService,
            IBus bus, // Passando o IBus do Rebus
            CancellationToken ct)
        {
            foreach (var donation in donations)
            {
                try 
                {
                    WalletAddress targetWallet = pendingStatus == DonationStatus.TransferredToVendorPending 
                        ? donation.TransferredToVendor!.Wallet 
                        : donation.Goal.Project.Manager.WalletAddress;

                    _logger.LogDebug("Consultando Blockchain para transação {Hash}...", donation.TransactionHash);

                    var isMoneyDonation = pendingStatus == DonationStatus.ImmediateTransferToNGOPending;

                    var currentStatus = await ethService.GetDonationStatus(
                        donation.TransactionHash, 
                        targetWallet, 
                        donation.Amount, 
                        isMoneyDonation
                    );

                    if (currentStatus != pendingStatus)
                    {
                        // Rebus: O método Publish funciona da mesma forma
                        await bus.Publish(new DonationStatusUpdatedMessage(
                            donation.Id, 
                            currentStatus, 
                            donation.TransactionHash
                        ));

                        _logger.LogInformation("Evento enviado ao RabbitMQ: Doação {Id} mudou para {Status} no Blockchain.", 
                            donation.Id, currentStatus);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao validar doação {Id} via serviço de Blockchain.", donation.Id);
                }
            }
        }
    }
}