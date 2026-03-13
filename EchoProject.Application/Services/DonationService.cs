using EchoProject.Application.Common;
using EchoProject.Application.DTO;
using EchoProject.Application.Exception;
using EchoProject.Application.Requests.Donation;
using EchoProject.Domain.DonationAggregate;
using EchoProject.Domain.Interfaces;
using EchoProject.Domain.UserAggregate;
using EchoProject.Infrastructure.Blockchain.Interfaces;

namespace EchoProject.Application.Services
{
    [AppService]
    public class DonationService(IUnitOfWork unitOfWork, IEthereumService ethereumService) 
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IEthereumService _ethereumService = ethereumService;

        public async Task<bool> DonateAsync(DonationRequest request, UserDTO donor)
        {
            if (donor.Role != UserRole.Donor)
                throw new UnauthorizedException("Only users with the Donor role can make donations.");

            var goal = await _unitOfWork.Goals.FindByIdAsync(request.GoalId)
                ?? throw new NotFoundException($"Goal with ID {request.GoalId} not found.");
            
            //TODO ETH service here 
            var donation = new Donation(donor.Id, goal, request.GoalId, request.Amount, "TransactionId", request.TotalAmount);

            await _unitOfWork.Donations.AddAsync(donation);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}