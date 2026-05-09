using EchoProject.Domain.Notifications;

namespace EchoProject.Domain.Repositories
{
    public interface IPushDeviceRepository : IRepository<PushDevice>
    {
        Task<PushDevice?> FindByTokenAsync(string token, CancellationToken ct = default);
        Task<List<PushDevice>> FindActiveByUserIdAsync(Guid userId, CancellationToken ct = default);
    }
}
