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
                ?? throw new NotFoundException($"Goal with ID {request.GoalId} not found.");

            bool isTransactionValid = await _ethereum.VerifyTransactionAsync(
                request.TransactionHash,
                goal.Project.SmartContractAddress,
                request.TotalAmount
            );

            if (!isTransactionValid)
            {
                _logger.LogWarning("Invalid blockchain transaction attempt. Hash: {Hash}", request.TransactionHash);
                throw new DomainException("The blockchain transaction is invalid, has failed, or the data does not match.");
            }

            var donation = new Donation(donor.Id, goal, request.Amount, request.TotalAmount, request.TransactionHash);

            try
            {
                await _unitOfWork.Donations.AddAsync(donation);
                // Saves a timeline of the donation for auditing purposes
                await _unitOfWork.DonationEvents.AddAsync(DonationEventFactory.Create(donation, donation.Status));
                goal.RegisterDonation(request.Amount);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the donation to the database.");
                throw new ApplicationException("Donation verified on blockchain but failed to save in database.", ex);
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
            return _unitOfWork.DonationEvents
                .FindAll(x => x.Donation.DonorId == user.Id && x.DonationId == donationId)
                .ToList()
                .DistinctBy(x => x.Status)
                .Select(_mapper.Map<DonationEventDTO>)
                .ToList();
        }

        public async Task<DonationDTO> GetByIdAsync(Guid donationId, UserDTO user)
        {
            var donation = await _unitOfWork.Donations.FindByIdAsync(donationId)
                ?? throw new NotFoundException($"Donation with ID {donationId} not found.");

            if (donation.DonorId != user.Id && user.Role != UserRole.EchoAdmin)
            {
                throw new UnauthorizedException("You are not the donor of this donation.");
            }

            return _mapper.Map<DonationDTO>(donation);
        }

        public async Task<PaginatedList<DonationDTO>> FindByProjectAsync(Guid projectId, PageRequest pr, UserDTO user)
        {
            var project = await _unitOfWork.Projects.FindByIdAsync(projectId)
                ?? throw new NotFoundException($"Project with ID {projectId} not found.");

            if (project.ManagerId != user.Id && user.Role != UserRole.EchoAdmin)
            {
                throw new UnauthorizedException("You are not the manager of this project.");
            }

            return _unitOfWork.Donations
                .FindByProjectId(projectId, CancellationToken.None)
                .Paginate(pr.PageNumber, pr.PageSize)
                .Select(x => _mapper.Map<DonationDTO>(x));
        }

        public async Task<bool> AssignDonationToVendorAsync(Guid donId, Guid vendorId, UserDTO user)
        {
            var donation = await _unitOfWork.Donations.FindByIdAsync(donId)
                ?? throw new NotFoundException($"Donation with ID {donId} not found.");

            var vendor = await _unitOfWork.Vendors.FindByIdAsync(vendorId)
                ?? throw new NotFoundException($"Vendor with ID {vendorId} not found.");

            if (donation.Goal.Project.ManagerId != user.Id)
            {
                throw new UnauthorizedException("You are not the manager of this project.");
            }

            donation.TransferToVendor(vendor);
            var project = donation.Goal.Project;

            _logger.LogInformation("Beginning transfer of funds to vendor. Donation ID: {DonationId}, Vendor ID: {VendorId}, Amount: {Amount}",
                donId, vendorId, donation.Amount);
            
            var finalTransactionHash = await _ethereum.ReleaseFundsToSupplierAsync(project.SmartContractAddress, vendor.Wallet, donation.TotalCost);
            
            donation.SetFundsReleasedHash(finalTransactionHash);
            await _unitOfWork.DonationEvents.AddAsync(DonationEventFactory.Create(donation, donation.Status));
            
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<Dictionary<string,decimal>> GetGlobalDonationDistributionPerGoalTypeAsync(int topN) 
            => await _unitOfWork.GoalTypes.GetTrendingGoalTypes(topN);
        
    }
}