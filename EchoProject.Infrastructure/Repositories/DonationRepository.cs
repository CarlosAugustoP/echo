using EchoProject.Domain.DonationAggregate;
using EchoProject.Domain.Repositories;
using EchoProject.Domain.UserAggregate;
using EchoProject.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EchoProject.Infrastructure.Repositories
{
    public class DonationRepository(EchoDbContext context) : EfRepository<Donation>(context), IDonationRepository
    {
        protected override IQueryable<Donation> Query => 
            base.Query
                .Include(x => x.Goal)
                .ThenInclude(g => g.Project)
                .Include(x => x.Goal)
                .ThenInclude(g => g.GoalType)
                .Include(x => x.Goal)
                .ThenInclude(g => g.Vendors)
                .Include(x => x.TransferredToVendor);

        public IEnumerable<Donation> FindByProjectId(Guid projectId, CancellationToken ct = default)
        {
            return Query.Where(d => d.Goal.ProjectId == projectId);
        }

        public IEnumerable<Donation> FindByUserIdAndProjectId(Guid userId, Guid projectId, CancellationToken ct = default)
        {
            return Query.Where(d => d.DonorId == userId && d.Goal.ProjectId == projectId);
        }

        public IEnumerable<Donation> FindUserHistory(Guid userId, CancellationToken ct = default)
        {
            return Query.Where(d => d.DonorId == userId);
        }

        public IEnumerable<Donation> FindPendingConfirmations(CancellationToken ct = default)
        {
            return Query.Where(d => d.Status == DonationStatus.TransferredToVendorPending);
        }

        public IEnumerable<Donation> FindDirectPendingNGOLiberation(CancellationToken ct = default)
        {
            return Query
                .Include(x => x.Goal)
                .ThenInclude(g => g.Project)
                .ThenInclude(g => g.Manager)
                .Where(x => x.Status == DonationStatus.ImmediateTransferToNGOInContract);
        }
    }
}