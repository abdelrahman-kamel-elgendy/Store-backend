using Microsoft.EntityFrameworkCore;
using Store.Core.Entities.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace Store.Infrastructure.Data.Config
{
    public class ProductConfigration : IEntityTypeConfiguration<Product>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Product> builder)
        {
            builder.Property(product => product.Id).IsRequired();
            builder.Property(product => product.Name).IsRequired().HasMaxLength(30);
            builder.Property(product => product.Price).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(product => product.Description).IsRequired().HasMaxLength(500);
            builder.HasData(new Product { Id = 1, Name = "Test", Description = "Test Product", CategoryId = 1, Price = 12});
        }
    }
}
