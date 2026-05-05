using EchoProject.Domain.Repositories;
using EchoProject.Domain.ValueObjects;
using EchoProject.Domain.VendorAggregate;
using EchoProject.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EchoProject.Infrastructure.Repositories
{
    public class VendorRepository(EchoDbContext context) : EfRepository<Vendor>(context), IVendorRepository
    {
        public async Task<Vendor?> FindByTaxIdAsync(TaxId taxId, CancellationToken ct = default)
        {
            return await Query.FirstOrDefaultAsync(v => v.Document.Value == taxId.Value, ct);
        }
    };
}
