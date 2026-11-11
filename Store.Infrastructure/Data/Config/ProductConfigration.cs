using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Core.Entities;

namespace Store.Infrastructure.Data.Config;

public class ProductConfigration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(product => product.Name).IsRequired().HasMaxLength(100);
        builder.Property(product => product.NewPrice).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(product => product.OldPrice).HasColumnType("decimal(18,2)");
        builder.Property(product => product.Description).HasMaxLength(1000);
        builder.HasIndex(product => product.CategoryId);

        builder.HasOne(product => product.Category)
            .WithMany(category => category.Products)
            .HasForeignKey(product => product.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
