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
    }
}