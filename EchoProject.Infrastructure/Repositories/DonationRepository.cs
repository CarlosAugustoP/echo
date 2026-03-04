using EchoProject.Domain.DonationAggregate;
using EchoProject.Domain.Repositories;
using EchoProject.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EchoProject.Infrastructure.Repositories
{
    public class DonationRepository(EchoDbContext context) : EfRepository<Donation>(context), IDonationRepository
    {
        public IEnumerable<Donation> FindByProjectIdAsync(Guid projectId, CancellationToken ct = default)
        {
            return _model.Include(x => x.Goal).Where(d => d.Goal.ProjectId == projectId);
        }

        public IEnumerable<Donation> FindByUserIdAndProjectIdAsync(Guid userId, Guid projectId, CancellationToken ct = default)
        {
            return _model.Include(x => x.Goal).Where(d => d.DonorId == userId && d.Goal.ProjectId == projectId);
        }

        public IEnumerable<Donation> FindUserHistory(Guid userId, CancellationToken ct = default)
        {
            return _model.Include(x => x.Goal).Where(d => d.DonorId == userId);
        }

    }
}