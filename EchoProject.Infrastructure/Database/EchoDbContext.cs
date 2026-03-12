using EchoProject.Domain.DonationAggregate;
using EchoProject.Domain.ProjectAggregate;
using EchoProject.Domain.UserAggregate;
using EchoProject.Domain.VendorAggregate;
using Microsoft.EntityFrameworkCore;

namespace EchoProject.Infrastructure.Database
{
    public class EchoDbContext(DbContextOptions<EchoDbContext> options) : DbContext(options)
    {
        public required DbSet<User> Users { get; set; }
        public required DbSet<Donation> Donations { get; set; }
        public required DbSet<Goal> Goals { get; set; }
        public required DbSet<Project> Projects { get; set; }
        public required DbSet<Vendor> Vendors { get; set; }
        public required DbSet<GoalType> GoalTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(EchoDbContext).Assembly);
            
            base.OnModelCreating(modelBuilder);
        }
    } 
}