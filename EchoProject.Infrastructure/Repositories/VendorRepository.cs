using EchoProject.Domain.Repositories;
using EchoProject.Domain.VendorAggregate;
using EchoProject.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EchoProject.Infrastructure.Repositories
{
    public class VendorRepository(EchoDbContext context) : EfRepository<Vendor>(context), IVendorRepository;
}