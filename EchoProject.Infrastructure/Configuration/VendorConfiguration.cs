using EchoProject.Domain.ValueObjects;
using EchoProject.Domain.VendorAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EchoProject.Infrastructure.Configuration
{
    public class VendorConfiguration : IEntityTypeConfiguration<Vendor>
    {
        public void Configure(EntityTypeBuilder<Vendor> builder)
{
    builder.ToTable("vendors");
    builder.HasKey(v => v.Id);

    builder.Property(v => v.Name)
        .HasColumnName("name")
        .IsRequired()
        .HasMaxLength(200);

    builder.Property(v => v.Status)
        .HasColumnName("status")
        .HasConversion<int>()
        .IsRequired();

    builder.Property(v => v.ApprovedById)
        .HasColumnName("approved_by_id")
        .IsRequired(false);

    builder.HasOne(p => p.ApprovedBy)
        .WithMany()
        .HasForeignKey(v => v.ApprovedById)
        .OnDelete(DeleteBehavior.Restrict);

    // Mapeamento corrigido: Apenas uma definição para Document
    builder.Property(v => v.Document)
        .HasColumnName("tax_id")
        .HasConversion(
            taxId => taxId.Value, 
            value => new TaxId(value)
        )
        .HasMaxLength(14)
        .IsRequired();

    // Adicione o índice separadamente, já que não estamos mais usando OwnsOne
    builder.HasIndex("Document").IsUnique(); 

    builder.Property(v => v.TypeItemSupply)
        .HasColumnName("type_item_supply")
        .IsRequired()
        .HasMaxLength(100);

    // Wallet mantido como OwnsOne, pois parece ser um objeto mais complexo
    builder.OwnsOne(v => v.Wallet, w =>
    {
        w.Property(p => p.Address).HasColumnName("wallet_address").HasMaxLength(42).IsRequired();
    });
}
    }
}