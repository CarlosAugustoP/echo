using EchoProject.Domain.DonationAggregate;
using EchoProject.Domain.Repositories;
using EchoProject.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EchoProject.Infrastructure.Repositories
{
    public class DonationEventRepository(EchoDbContext context) : EfRepository<DonationEvent>(context), IDonationEventRepository
    {
    }
}