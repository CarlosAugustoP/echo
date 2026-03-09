using EchoProject.Domain.ProjectAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EchoProject.Infrastructure.Configuration
{
    public class GoalConfiguration : IEntityTypeConfiguration<Goal>
    {
        public void Configure(EntityTypeBuilder<Goal> builder)
        {
            builder.ToTable("goals");
            builder.HasKey(g => g.Id);

            builder.Property(g => g.Title)
                .HasColumnName("title")
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(g => g.TargetAmount)
                .HasColumnName("target_amount")
                .IsRequired();

            builder.Property(g => g.CurrentAmount)
                .HasColumnName("current_amount")
                .IsRequired()
                .HasDefaultValue(0);

            builder.HasOne(g => g.Project)
                .WithMany(p => (IEnumerable<Goal>?)p.Goals)
                .HasForeignKey(g => g.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("project_id");

            builder.Property(g => g.ProjectId)
                .HasColumnName("project_id")
                .IsRequired();

            builder.HasOne(g => g.GoalType)
                .WithMany()
                .HasForeignKey(g => g.GoalTypeId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("goal_type_id");

            builder.Property(g => g.GoalTypeId)
                .HasColumnName("goal_type_id")
                .IsRequired();

            builder.HasMany(g => g.Vendors)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "goal_vendor", // Name of the join table
                    j => j.HasOne<Domain.VendorAggregate.Vendor>().WithMany().HasForeignKey("vendor_id"),
                    j => j.HasOne<Goal>().WithMany().HasForeignKey("goal_id")
                );

            builder.Navigation(g => g.Vendors)
                .HasField("_vendors")
                .UsePropertyAccessMode(PropertyAccessMode.PreferField);
        }
    }
}