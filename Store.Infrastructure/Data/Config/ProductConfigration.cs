using Microsoft.EntityFrameworkCore;
using Store.Core.Entities.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace Store.Infrastructure.Data.Config
{
    /// <summary>Configures the product schema and its initial seed records.</summary>
    public class ProductConfigration : IEntityTypeConfiguration<Product>
    {
        /// <summary>Defines product field constraints and seed data.</summary>
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Product> builder)
        {
            builder.Property(product => product.Id).IsRequired();
            builder.Property(product => product.Name).IsRequired().HasMaxLength(30);
            builder.Property(product => product.Price).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(product => product.Description).IsRequired().HasMaxLength(500);

            builder.HasData(
                new Product
                {
                    Id = 1,
                    Name = "Test",
                    Description = "Test Product",
                    CategoryId = 1,
                    Price = 12
                },
                new Product
                {
                    Id = 2,
                    Name = "Test_2",
                    Description = "Test Product 2",
                    CategoryId = 2,
                    Price = 10
                });
        }
    }
}
