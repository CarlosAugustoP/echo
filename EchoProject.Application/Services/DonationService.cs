using AutoMapper;
using EchoProject.Application.Common;
using EchoProject.Application.Common.PaginatedList;
using EchoProject.Application.DTO;
using EchoProject.Application.DTO.Donations;
using EchoProject.Application.Exceptions;
using EchoProject.Application.Requests.Donation;
using EchoProject.Application.Requests.Pagination;
using EchoProject.Domain.DonationAggregate;
using EchoProject.Domain.Exception.EchoProject.Domain.Common;
using EchoProject.Domain.Interfaces;
using EchoProject.Domain.Notifications;
using EchoProject.Domain.ProjectAggregate;
using EchoProject.Domain.UserAggregate;
using EchoProject.Infrastructure.Blockchain.Interfaces;
using Microsoft.Extensions.Logging;

namespace EchoProject.Application.Services
{
    [AppService]
    public class DonationService(IUnitOfWork unitOfWork, IEthereumService ethereumService, ILogger<DonationService> logger, IMapper mapper)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IEthereumService _ethereum = ethereumService;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<DonationService> _logger = logger;
        
        public async Task<bool> DonateAsync(DonationRequest request, UserDTO donor)
        {
            var goal = await _unitOfWork.Goals.FindByIdAsync(request.GoalId)
                ?? throw new NotFoundException($"Meta com ID {request.GoalId} não encontrada.");

            bool isTransactionValid = await _ethereum.VerifyTransactionAsync(
                request.TransactionHash,
                goal.Project.SmartContractAddress,
                request.TotalAmountETH
            );

            if (!isTransactionValid)
            {
                _logger.LogWarning("Invalid blockchain transaction attempt. Hash: {Hash}", request.TransactionHash);
                throw new DomainException("A transação na blockchain é inválida, falhou ou os dados não conferem.");
            }

            var donation = new Donation(donor.Id, goal, request.Amount, request.TotalAmountETH, request.TransactionHash);

            try
            {
                await _unitOfWork.Donations.AddAsync(donation);
                // Saves a timeline of the donation for auditing purposes
                var donationEvent = donation.AddEvent(donation.Status);
                
                _unitOfWork.Donations.AddDonationEvent(donationEvent);

                var notificationRequest = donation.GetNotificationRequest();
                if (notificationRequest is not null)
                {
                    foreach (var notification in NotificationFactory.Create(notificationRequest.Type, notificationRequest.Model))
                    {
                        await _unitOfWork.Notifications.AddAsync(notification);
                    }
                }

                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the donation to the database.");
                throw new ApplicationException("A doação foi verificada na blockchain, mas não pôde ser salva no banco de dados.", ex);
            }

            return true;
        }

        public PaginatedList<DonationDTO> GetHistoryByDonorId(Guid userId, PageRequest pr)
        {
            return _unitOfWork.Donations
                .FindUserHistory(userId, CancellationToken.None)
                .OrderByDescending(x => x.CreatedAt)
                .Paginate(pr.PageNumber, pr.PageSize)
                .Select(x => _mapper.Map<DonationDTO>(x));
        }

        public List<DonationEventDTO> GetTimeline(UserDTO user, Guid donationId)
        {
            return _unitOfWork.Donations.FindDonationEventsByDonationId(donationId, CancellationToken.None)
                .Where(x => x.Donation.DonorId == user.Id)
                .ToList()
                .DistinctBy(x => x.Status)
                .Select(_mapper.Map<DonationEventDTO>)
                .ToList();
        }

        public async Task<DonationDTO> GetByIdAsync(Guid donationId, UserDTO user)
        {
            var donation = await _unitOfWork.Donations.FindByIdAsync(donationId)
                ?? throw new NotFoundException($"Doação com ID {donationId} não encontrada.");

            if (donation.DonorId != user.Id && user.Role != UserRole.EchoAdmin)
            {
                throw new UnauthorizedException("Você não é o doador desta doação.");
            }

            return _mapper.Map<DonationDTO>(donation);
        }

        public async Task<PaginatedList<DonationDTO>> FindByProjectAsync(Guid projectId, PageRequest pr, UserDTO user)
        {
            var project = await _unitOfWork.Projects.FindByIdAsync(projectId)
                ?? throw new NotFoundException($"Projeto com ID {projectId} não encontrado.");

            if (project.ManagerId != user.Id && user.Role != UserRole.EchoAdmin)
            {
                throw new UnauthorizedException("Você não é o gestor deste projeto.");
            }

            return _unitOfWork.Donations
                .FindByProjectId(projectId, CancellationToken.None)
                .Paginate(pr.PageNumber, pr.PageSize)
                .Select(x => _mapper.Map<DonationDTO>(x));
        }

        public async Task<bool> AssignDonationToVendorAsync(Guid donId, Guid vendorId, UserDTO user)
        {
            var donation = await _unitOfWork.Donations.FindByIdAsync(donId)
                ?? throw new NotFoundException($"Doação com ID {donId} não encontrada.");

            var vendor = await _unitOfWork.Vendors.FindByIdAsync(vendorId)
                ?? throw new NotFoundException($"Fornecedor com ID {vendorId} não encontrado.");

            if (donation.Goal.Project.ManagerId != user.Id)
            {
                throw new UnauthorizedException("Você não é o gestor deste projeto.");
            }

            donation.TransferToVendor(vendor);
            var project = donation.Goal.Project;

            _logger.LogInformation("Beginning transfer of funds to vendor. Donation ID: {DonationId}, Vendor ID: {VendorId}, Amount: {Amount}",
                donId, vendorId, donation.Amount);
            
            var finalTransactionHash = await _ethereum.ReleaseFundsToSupplierAsync(project.SmartContractAddress, vendor.Wallet, donation.TotalCost);
            
            donation.SetFundsReleasedHash(finalTransactionHash);
            var donationevent = donation.AddEvent(donation.Status);
            
            _unitOfWork.Donations.AddDonationEvent(donationevent);
            
            await _unitOfWork.CommitAsync();
            return true;
        }

        public PaginatedList<DonationDTO> GetPendingDonationsByProject(Guid projectId, PageRequest p, UserDTO user)
        {
            var project = _unitOfWork.Projects.FindByManager(user.Id).FirstOrDefault(p => p.Id == projectId)
                ?? throw new NotFoundException($"Projeto com ID {projectId} não encontrado ou você não o gerencia.");
            
            return _unitOfWork.Donations.FindPendingDonationsByProjectId(projectId)
                .Paginate(p.PageNumber,p.PageSize)
                .Select(x => _mapper.Map<DonationDTO>(x));
        }

        public async Task<Dictionary<string,decimal>> GetGlobalDonationDistributionPerGoalTypeAsync(int topN) 
            => await _unitOfWork.Goals.GetTrendingGoalTypes(topN);

       
        
    }
}
