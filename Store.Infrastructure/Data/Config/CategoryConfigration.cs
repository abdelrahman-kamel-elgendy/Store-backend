using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Core.Entities.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace Store.Infrastructure.Data.Config
{
    public class CategoryConfigration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(category => category.Name).IsRequired().HasMaxLength(100); 
            builder.Property(category => category.Id).IsRequired().HasMaxLength(500);
            builder.HasData(new Category { Id = 1, Name = "Test", Description = "Test Category"});
        }
    }
}
