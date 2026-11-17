using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Core.Entities;

namespace Store.Infrastructure.Data.Configurations;

/// <summary>Relational mapping for <see cref="Category"/>.</summary>
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    /// <summary>Configures columns, indexes, and the parent/child relationship.</summary>
    /// <param name="builder">The entity builder.</param>
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.Property(category => category.Name).IsRequired().HasMaxLength(100);
        builder.Property(category => category.Description).HasMaxLength(500);
        builder.HasIndex(category => category.ParentCategoryId);
        builder.HasIndex(category => category.Name);

        builder.HasOne(category => category.ParentCategory)
            .WithMany(category => category.SubCategories)
            .HasForeignKey(category => category.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(category => category.Products)
            .WithOne(product => product.Category)
            .HasForeignKey(product => product.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
