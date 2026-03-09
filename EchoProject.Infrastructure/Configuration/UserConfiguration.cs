    using EchoProject.Domain.UserAggregate;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    namespace EchoProject.Infrastructure.Configuration
    {
        public class UserConfiguration : IEntityTypeConfiguration<User>
        {
            public void Configure(EntityTypeBuilder<User> builder)
            {
                builder.ToTable("users");
                builder.HasKey(u => u.Id);

                builder.Property(u => u.Name)
                    .HasColumnName("name")
                    .IsRequired()
                    .HasMaxLength(150);

                builder.Property(u => u.Email)
                    .HasColumnName("email")
                    .IsRequired()
                    .HasMaxLength(100);
                    
                builder.Property(u => u.PasswordHash)
                    .HasColumnName("password_hash")
                    .IsRequired()
                    .HasMaxLength(255);
                
                builder.HasIndex(u => u.Email).IsUnique();

                builder.Property(u => u.Role)
                    .HasColumnName("role")
                    .HasConversion<int>()
                    .IsRequired();

                builder.OwnsOne(u => u.TaxId, t =>
                {
                    t.Property(p => p.Value).HasColumnName("tax_id").HasMaxLength(14).IsRequired();
                });

                builder.OwnsOne(u => u.WalletAddress, w =>
                {
                    w.Property(p => p.Address).HasColumnName("wallet_address").HasMaxLength(42).IsRequired();
                });

                builder.OwnsOne(u => u.Address, a =>
                {
                    a.Property(p => p.Street).HasColumnName("street").HasMaxLength(200);
                    a.Property(p => p.City).HasColumnName("city").HasMaxLength(100);
                    a.Property(p => p.State).HasColumnName("state").HasMaxLength(2);
                    a.Property(p => p.PostCode).HasColumnName("zip_code").HasMaxLength(20);
                    a.Property(p => p.CountryCode).HasColumnName("country_code").HasMaxLength(2);
                    a.Property(p => p.Neighborhood).HasColumnName("neighbourhood").HasMaxLength(100);
                });
            }
        }
    }