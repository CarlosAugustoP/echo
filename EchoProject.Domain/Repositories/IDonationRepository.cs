using EchoProject.Domain.DonationAggregate;

namespace EchoProject.Domain.Repositories
{
    public interface IDonationRepository : IRepository<Donation>
    {
        IEnumerable<Donation> FindByUserIdAndProjectId(Guid userId, Guid projectId, CancellationToken ct = default);
        IEnumerable<Donation> FindByProjectId(Guid projectId, CancellationToken ct = default);
        IEnumerable<Donation> FindUserHistory(Guid userId, CancellationToken ct = default);
    }
}