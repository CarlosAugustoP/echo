using EchoProject.Domain.ValueObjects;
using EchoProject.Domain.VendorAggregate;

namespace EchoProject.Domain.Repositories
{
    public interface IVendorRepository : IRepository<Vendor>
    {
        Task<Vendor?> FindByTaxIdAsync(TaxId taxId, CancellationToken ct = default);
    }
}
