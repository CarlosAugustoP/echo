using EchoProject.Domain.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EchoProject.Infrastructure.Configuration
{
    public class PushDeviceConfiguration : IEntityTypeConfiguration<PushDevice>
    {
        public void Configure(EntityTypeBuilder<PushDevice> builder)
        {
            builder.ToTable("push_devices");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id");

            builder.Property(x => x.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.Property(x => x.Token)
                .HasColumnName("token")
                .HasMaxLength(512)
                .IsRequired();

            builder.Property(x => x.Platform)
                .HasColumnName("platform")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .HasColumnName("updated_at")
                .IsRequired();

            builder.Property(x => x.LastUsedAt)
                .HasColumnName("last_used_at");

            builder.Property(x => x.IsActive)
                .HasColumnName("is_active")
                .IsRequired();

            builder.HasIndex(x => x.Token)
                .IsUnique();

            builder.HasIndex(x => new { x.UserId, x.IsActive });

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
