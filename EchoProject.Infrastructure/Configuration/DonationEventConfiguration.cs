using EchoProject.Domain.DonationAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EchoProject.Infrastructure.Configuration
{
    public class DonationEventConfiguration : IEntityTypeConfiguration<DonationEvent>
    {
        public void Configure(EntityTypeBuilder<DonationEvent> builder)
        {
            builder.ToTable("donation_events");

            builder.HasKey(de => de.Id);

            builder.Property(de => de.Id)
                .HasColumnName("id");

            builder.Property(de => de.DonationId)
                .HasColumnName("donation_id")
                .IsRequired();

            builder.Property(de => de.Timestamp) 
                .HasColumnName("timestamp")
                .IsRequired();

            builder.Property(de => de.Status)
                .HasColumnName("status")
                .HasConversion<int>() 
                .IsRequired();

            builder.Property(de => de.Message)
                .HasColumnName("message")
                .HasMaxLength(500);

            builder.HasOne(de => de.Donation)
                .WithMany(d => d.Events) 
                .HasForeignKey(de => de.DonationId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}