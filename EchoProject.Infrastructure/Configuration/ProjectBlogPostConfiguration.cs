using System.Text.Json;
using EchoProject.Domain.ProjectAggregate;
using EchoProject.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EchoProject.Infrastructure.Configuration
{
    public class ProjectBlogPostConfiguration : IEntityTypeConfiguration<ProjectBlogPost>
    {
        public void Configure(EntityTypeBuilder<ProjectBlogPost> builder)
        {
            builder.ToTable("project_blog_posts");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Content)
                .HasColumnName("content")
                .IsRequired()
                .HasColumnType("text");

            builder.Property(x => x.ProjectId)
                .HasColumnName("project_id");

            builder.OwnsOne(x => x.HeaderImage, cb =>
            {
                cb.Property(p => p.Url)
                    .HasColumnName("header_image_url")
                    .IsRequired();
            });

            var imageUrlComparer = new ValueComparer<IReadOnlyCollection<ImageUrl>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList());

            builder.Property(x => x.Images)
               .HasConversion(
                   v => v.Select(img => img.Url).ToArray(),

                   v => v.Select(url => new ImageUrl(url)).ToList()
               )
               .HasColumnName("additional_images")
               .HasColumnType("text[]")
               .Metadata.SetValueComparer(imageUrlComparer);

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(x => x.Project)
                .WithMany(p => p.BlogPosts)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            var navigation = builder.Metadata.FindNavigation(nameof(ProjectBlogPost.Images));
            navigation?.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}