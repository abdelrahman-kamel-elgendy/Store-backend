using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Core.Entities;

namespace Store.Infrastructure.Data.Config;

public class CategoryConfigration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.Property(category => category.Name).IsRequired().HasMaxLength(100);
        builder.Property(category => category.Description).HasMaxLength(500);
        builder.HasIndex(category => category.ParentCategoryId);
        builder.HasIndex(category => category.Name).IsUnique(false);

        builder.HasOne(c => c.ParentCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(category => category.SubCategories)
            .WithOne(category => category.ParentCategory)
            .HasForeignKey(category => category.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(category => category.Products)
            .WithOne(product => product.Category)
            .HasForeignKey(product => product.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
