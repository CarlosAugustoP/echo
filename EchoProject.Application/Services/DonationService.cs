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
using EchoProject.Domain.UserAggregate;
using EchoProject.Infrastructure.Blockchain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EchoProject.Application.Services
{
    [AppService]
    public class DonationService(IUnitOfWork unitOfWork, IEthereumService ethereumService, IConfiguration conf, ILogger<DonationService> logger, IMapper mapper)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IEthereumService _ethereumService = ethereumService;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<DonationService> _logger = logger;
        private readonly string _contractAddress = conf["Blockchain:ContractAddress"]
            ?? throw new ArgumentException("Contract address is not configured.");

        public async Task<bool> DonateAsync(DonationRequest request, UserDTO donor)
        {
            if (donor.Role != UserRole.Donor)
                throw new UnauthorizedException("Only users with the Donor role can make donations.");

            var goal = await _unitOfWork.Goals.FindByIdAsync(request.GoalId)
                ?? throw new NotFoundException($"Goal with ID {request.GoalId} not found.");

            bool isTransactionValid = await _ethereumService.VerifyTransactionAsync(
                request.TransactionHash,
                _contractAddress,
                request.Amount
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
        public PaginatedList<DonationDTO> GetByDonorId(Guid userId, PageRequest pr)
        {
            return _unitOfWork.Donations
                .FindUserHistory(userId, CancellationToken.None)
                .Paginate(pr.PageNumber, pr.PageSize)
                .Select(x => _mapper.Map<DonationDTO>(x));
        }

        public async Task<PaginatedList<DonationDTO>> FindByProject(Guid projectId, PageRequest pr, UserDTO user)
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
    }
}