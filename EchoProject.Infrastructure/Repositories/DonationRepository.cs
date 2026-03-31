using EchoProject.Domain.DonationAggregate;
using EchoProject.Domain.Repositories;
using EchoProject.Domain.UserAggregate;
using EchoProject.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EchoProject.Infrastructure.Repositories
{
    public class DonationRepository(EchoDbContext context) : EfRepository<Donation>(context), IDonationRepository
    {
        private readonly EchoDbContext _db = context;
        protected override IQueryable<Donation> Query =>
            base.Query
                .Include(x => x.Goal)
                .ThenInclude(g => g.Project)
                .Include(x => x.Goal)
                .ThenInclude(g => g.GoalType)
                .Include(x => x.Goal)
                .ThenInclude(g => g.Vendors)
                .Include(x => x.TransferredToVendor);

        public IQueryable<Donation> FindByProjectId(Guid projectId, CancellationToken ct = default)
        {
            return Query.Where(d => d.Goal.ProjectId == projectId);
        }

        public IQueryable<Donation> FindByUserIdAndProjectId(Guid userId, Guid projectId, CancellationToken ct = default)
        {
            return Query.Where(d => d.DonorId == userId && d.Goal.ProjectId == projectId);
        }

        public IQueryable<Donation> FindUserHistory(Guid userId, CancellationToken ct = default)
        {
            return Query.Where(d => d.DonorId == userId);
        }

        public IQueryable<Donation> FindPendingConfirmations(CancellationToken ct = default)
        {
            return Query.Where(d => d.Status == DonationStatus.TransferredToVendorPending);
        }

        public IQueryable<Donation> FindDirectPendingNGOLiberation(CancellationToken ct = default)
        {
            return Query
                .Include(x => x.Goal)
                .ThenInclude(g => g.Project)
                .ThenInclude(g => g.Manager)
                .Where(x => x.Status == DonationStatus.ImmediateTransferToNGOInContract);
        }
        public (decimal TotalContributedThisMonth, decimal TotalContributedLastMonth) FindContributionSummaryAsync(Guid userId, CancellationToken ct = default)
        {
            var donationsByUser = Query.Where(u => u.DonorId == userId);

            var totalContributedThisMonth = donationsByUser
                .Where(d => d.CreatedAt >= DateTime.UtcNow.AddMonths(-1))
                .Sum(d => (decimal?)d.TotalCost) ?? 0;

            var totalContributedLastMonth = donationsByUser
                .Where(d => d.CreatedAt >= DateTime.UtcNow.AddMonths(-2) && d.CreatedAt < DateTime.UtcNow.AddMonths(-1))
                .Sum(d => (decimal?)d.TotalCost) ?? 0;

            return (totalContributedThisMonth, totalContributedLastMonth);
        }

        public async Task<List<(string GoalType, int Count)>> FindDonationCountByGoalTypeForUserAsync(Guid userId, CancellationToken ct = default)
        {
            var initialQuery = await Query.Where(c => c.DonorId == userId)
                .GroupBy(x => x.Goal.GoalType.Name)
                .Select(x => new { GoalType = x.Key, Count = x.Count() })
                .ToListAsync();
            
            return initialQuery.Select(x => (x.GoalType, x.Count)).ToList();
        }

        public async Task<List<(string CountryCode, string StateCode, decimal Amount)>> FindImpactByRegionForProjectAsync(Guid projectId, CancellationToken ct = default)
        {
            var initialQuery = await Query.Where(d => d.Goal.ProjectId == projectId)
                .Include(x => x.Goal.Project.Manager)
                .GroupBy(d => new { d.Goal.Project.Manager.Address.CountryCode, d.Goal.Project.Manager.Address.State })
                .Select(g => new 
                {
                    g.Key.CountryCode, 
                    StateCode = g.Key.State, 
                    Amount = g.Sum(d => (decimal?)d.TotalCost) ?? 0 
                })
                .ToListAsync(cancellationToken: ct);
            
            return initialQuery.Select(x => (x.CountryCode, x.StateCode, x.Amount)).ToList();
        }

        public IQueryable<DonationEvent> FindDonationEventsByUserId(Guid userId, CancellationToken ct = default)
        {
            return _db.DonationEvents.Include(x => x.Donation).Where(de => de.Donation.DonorId == userId);
        }

        public void AddDonationEvent(DonationEvent donationEvent)
        {
            _db.DonationEvents.Add(donationEvent);  
        }

        public IQueryable<DonationEvent> FindDonationEventsByDonationId(Guid donationId, CancellationToken ct = default)
        {
            return _db.DonationEvents
                .Include(x => x.Donation)
                .ThenInclude(d => d.Goal)
                .Where(de => de.Donation.Id == donationId);
        }
    }
}