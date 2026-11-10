using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Core.Entities;

namespace Store.Infrastructure.Data.Config;

public class PhotoConfigration : IEntityTypeConfiguration<Photo>
{
    public void Configure(EntityTypeBuilder<Photo> builder)
    {
        builder.Property(photo => photo.Name).IsRequired().HasMaxLength(200);
        builder.Property(photo => photo.Url).IsRequired().HasMaxLength(500);
        builder.HasIndex(photo => photo.ProductId);
        builder.HasIndex(photo => new { photo.ProductId, photo.IsMain });
    }
}
