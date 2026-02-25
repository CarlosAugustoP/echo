using EchoProject.Domain.ProjectAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EchoProject.Infrastructure.Configuration
{
    public class GoalTypeConfiguration : IEntityTypeConfiguration<GoalType>
    {
        public void Configure(EntityTypeBuilder<GoalType> builder)
        {
            builder.ToTable("goal_types");
            builder.HasKey(gt => gt.Id);

            builder.Property(gt => gt.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(gt => gt.Description)
                .HasColumnName("description")
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(gt => gt.IsActive)
                .HasColumnName("is_active")
                .IsRequired()
                .HasDefaultValue(true);
                
            builder.HasIndex(gt => gt.Name).IsUnique();
        }
    }
}