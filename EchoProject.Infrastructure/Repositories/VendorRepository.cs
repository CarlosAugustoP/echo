using EchoProject.Domain.Repositories;
using EchoProject.Domain.VendorAggregate;
using Microsoft.EntityFrameworkCore;

namespace EchoProject.Infrastructure.Repositories
{
    public class VendorRepository(DbContext context) : EfRepository<Vendor>(context), IVendorRepository;
}