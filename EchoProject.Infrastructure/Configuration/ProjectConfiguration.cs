using EchoProject.Domain.ProjectAggregate;
using EchoProject.Domain.UserAggregate;
using EchoProject.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EchoProject.Infrastructure.Configuration
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("projects");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Title)
                .HasColumnName("title")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Description)
                .HasColumnName("description")
                .IsRequired()
                .HasMaxLength(1000);

            builder.HasOne(p => p.Manager)
                .WithMany()
                .HasForeignKey(p => p.ManagerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("manager_id");

            builder.Property(p => p.ManagerId)
                .HasColumnName("manager_id");

            builder.Property(x => x.SmartContractAddress)
                .HasConversion(
                    v => v.Value,
                    v => new SmartContractAddress(v))
                .HasColumnName("smart_contract_address")
                .IsRequired()
                .HasMaxLength(42);

            builder.Property(x => x.MainImage)
                .HasConversion(
                    v => v != null ? v.Url : null,
                    v => v != null ? new ImageUrl(v) : null
                )
                .HasColumnName("main_image_url")
                .HasMaxLength(255);
            
            var imageUrlComparer = new ValueComparer<IReadOnlyCollection<ImageUrl>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList());

            builder.Property(x => x.Images)
                .HasConversion(
                    v => v.Select(img => img.Url).ToArray(),
                    v => v.Select(url => new ImageUrl(url)).ToList()
                )
                .HasColumnName("images")
                .HasColumnType("text[]")
                .Metadata.SetValueComparer(imageUrlComparer);   
            
            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Metadata.FindNavigation(nameof(Project.Goals))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.Navigation(x => x.BlogPosts)
                .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}