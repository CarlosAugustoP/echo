using EchoProject.Domain.DonationAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EchoProject.Infrastructure.Configuration
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("notifications");

            builder.HasKey(n => n.Id);

            builder.Property(n => n.Id)
                .HasColumnName("id");

            builder.Property(n => n.Message)
                .HasColumnName("message")
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(n => n.Description)
                .HasColumnName("description")
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(n => n.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(n => n.SentTo)
                .HasColumnName("sent_to")
                .IsRequired();

            builder.HasOne(n => n.SentToUser)
                .WithMany()
                .HasForeignKey(n => n.SentTo)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
