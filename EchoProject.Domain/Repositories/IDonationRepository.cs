using EchoProject.Domain.DonationAggregate;
using EchoProject.Domain.UserAggregate;

namespace EchoProject.Domain.Repositories
{
    public interface IDonationRepository : IRepository<Donation>
    {
        IEnumerable<Donation> FindByUserIdAndProjectId(Guid userId, Guid projectId, CancellationToken ct = default);
        IEnumerable<Donation> FindByProjectId(Guid projectId, CancellationToken ct = default);
        IEnumerable<Donation> FindUserHistory(Guid userId, CancellationToken ct = default);
        IEnumerable<Donation> FindPendingConfirmations(CancellationToken ct = default);
        IEnumerable<Donation> FindDirectPendingNGOLiberation(CancellationToken ct = default);
    }
}