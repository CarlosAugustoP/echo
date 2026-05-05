using EchoProject.Domain.VendorAggregate;

namespace EchoProject.Domain.Repositories
{
    public interface IVendorRepository : IRepository<Vendor>
    {
        Task<Vendor?> FindByTaxIdAsync(string taxId, CancellationToken ct = default);
    }
}
