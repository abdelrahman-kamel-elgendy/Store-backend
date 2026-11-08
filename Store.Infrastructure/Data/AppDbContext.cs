using Microsoft.EntityFrameworkCore;
using Store.Core.Entities.Product;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Store.Infrastructure.Data
{
    /// <summary>Represents the application's Entity Framework Core database session.</summary>
    public class AppDbContext : DbContext
    {
        /// <summary>Initializes the context with the configured database options.</summary>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        /// <summary>Gets the categories stored in the database.</summary>
        public virtual DbSet<Category> Categories { get; set; }
        /// <summary>Gets the products stored in the database.</summary>
        public virtual DbSet<Product> Products { get; set; }
        /// <summary>Gets the product photos stored in the database.</summary>
        public virtual DbSet<Photo> Photos { get; set; }

        /// <summary>Applies entity configurations discovered in the infrastructure assembly.</summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly()); // Apply all configurations from the current assembly
        }
    }
}
