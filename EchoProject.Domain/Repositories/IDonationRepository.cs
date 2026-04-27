using EchoProject.Domain.DonationAggregate;
using EchoProject.Domain.UserAggregate;

namespace EchoProject.Domain.Repositories
{
    public interface IDonationRepository : IRepository<Donation>
    {
        IQueryable<Donation> FindByUserIdAndProjectId(Guid userId, Guid projectId, CancellationToken ct = default);
        IQueryable<Donation> FindByProjectId(Guid projectId, CancellationToken ct = default);
        IQueryable<Donation> FindUserHistory(Guid userId, CancellationToken ct = default);
        IQueryable<Donation> FindPendingConfirmations(CancellationToken ct = default);
        IQueryable<Donation> FindDirectPendingNGOLiberation(CancellationToken ct = default);
        (decimal TotalContributedThisMonth, decimal TotalContributedLastMonth) FindContributionSummary(Guid userId, CancellationToken ct = default);
        Task<List<(string GoalType, int Count)>> FindDonationCountByGoalTypeForUserAsync(Guid userId, CancellationToken ct = default);
        Task<List<(string CountryCode, string StateCode, decimal Amount)>> FindImpactByRegionForUserAsync(Guid userId, CancellationToken ct = default);
        IQueryable<DonationEvent> FindDonationEventsByUserId(Guid userId, CancellationToken ct = default);
        IQueryable<DonationEvent> FindDonationEventsByDonationId(Guid donationId, CancellationToken ct = default);
        void AddDonationEvent(DonationEvent donationEvent);

    }
}