using AutoMapper;
using EchoProject.Application.Common;
using EchoProject.Application.DTO;
using EchoProject.Application.DTO.Dashboard;
using EchoProject.Application.DTO.Donations;
using EchoProject.Domain.DonationAggregate;
using EchoProject.Domain.Interfaces;

namespace EchoProject.Application.Services
{
    [AppService]
    public class DashboardService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        public readonly IMapper _mapper = mapper;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public ContributionSummaryDTO GetContributionSummary(UserDTO currentUser)
        {
            var (thisMonth, lastMonth) = _unitOfWork.Donations.FindContributionSummary(currentUser.Id);
            return new ContributionSummaryDTO(thisMonth, lastMonth == 0 ? 0 : (thisMonth - lastMonth) / lastMonth);
        }

        public async Task<List<AmountAndGoalTypeDTO>> GetAmountAndGoalTypeAsync(UserDTO currentUser)
        {
            var donationCounts = await _unitOfWork.Donations.FindDonationCountByGoalTypeForUserAsync(currentUser.Id);
            
            return 
            [
                .. donationCounts
                .OrderByDescending(dc => dc.Count)
                .Select(dc => new AmountAndGoalTypeDTO(dc.GoalType, dc.Count))
                .Take(7)
            ];
        }

        public async Task<List<ImpactByRegionDTO>> GetImpactByRegionAsync(Guid userId)
        {
            var impactData = await _unitOfWork.Donations.FindImpactByRegionForUserAsync(userId);
            return 
            [
                .. impactData
                .OrderByDescending(x => x.Amount)
                .Select(d => new ImpactByRegionDTO(d.CountryCode, d.StateCode, d.Amount))
                .Take(7)
            ];
        }

        public ContributionTotalDTO GetTotalDonationsCountByUserId(Guid userId) 
        {
            var u = _unitOfWork.Donations
                .FindAll(d => d.DonorId == userId);

            return new ContributionTotalDTO(u.Sum(d => d.TotalCost), u.Count());
        }

        public List<DonationEventDTO> GetDonationEvents(UserDTO currentUser)
        {
            var events = _unitOfWork.Donations.FindDonationEventsByUserId(currentUser.Id);
            
            return 
            [
                .. events
                .OrderByDescending(e => e.Timestamp)
                .Take(7)
                .ToList()
                .Select(e => _mapper.Map<DonationEventDTO>(e))
            ];
        }
    }
}