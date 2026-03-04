using EchoProject.Domain.DonationAggregate;
using EchoProject.Domain.Models;
using EchoProject.Domain.ProjectAggregate;
using EchoProject.Domain.UserAggregate;
using EchoProject.Domain.VendorAggregate;
using Microsoft.EntityFrameworkCore;

namespace EchoProject.Infrastructure.Database
{
    public class EchoDbContext(DbContextOptions<EchoDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<GoalType> GoalTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(EchoDbContext).Assembly);
            
            base.OnModelCreating(modelBuilder);
        }
    } 
}