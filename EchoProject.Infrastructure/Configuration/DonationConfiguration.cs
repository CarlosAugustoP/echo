using EchoProject.Domain.DonationAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EchoProject.Infrastructure.Configuration
{
    public class DonationConfiguration : IEntityTypeConfiguration<Donation>
    {
        public void Configure(EntityTypeBuilder<Donation> builder)
        {
            builder.ToTable("donations");

            builder.HasKey(d => d.Id);
            
            builder.Property(d => d.Id)
                .HasColumnName("id");

            builder.Property(d => d.DonorId)
                .HasColumnName("donor_id")
                .IsRequired();

            builder.Property(d => d.GoalId)
                .HasColumnName("goal_id")
                .IsRequired();

            builder.Property(d => d.Amount)
                .HasColumnName("amount")
                .HasColumnType("decimal(38,18)")
                .IsRequired();
            
            builder.Property(d => d.TotalCost)
                .HasColumnName("total_cost")
                .HasColumnType("decimal(38,18)")
                .IsRequired();

            builder.Property(d => d.TransactionHash)
                .HasColumnName("transaction_hash")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.Status)
                .HasColumnName("status")
                .HasConversion<int>() 
                .IsRequired();

            builder.Property(d => d.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.HasOne(d => d.Donor)
                .WithMany()
                .HasForeignKey(d => d.DonorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.Goal)
                .WithMany()
                .HasForeignKey(d => d.GoalId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(d => d.TransactionHash)
                .IsUnique();

            builder.HasOne(d => d.TransferredToVendor)
                .WithMany()
                .HasForeignKey(d => d.TransferredToVendorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(d => d.TransferredToVendorId)
                .HasColumnName("transferred_to_vendor_id")
                .IsRequired(false);
        }
    }
}