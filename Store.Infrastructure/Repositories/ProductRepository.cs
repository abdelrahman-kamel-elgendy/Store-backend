using Store.Core.Entities.Product;
using Store.Core.Interfaces;
using Store.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Store.Infrastructure.Repositories
{
    public class ProductRepository:GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }   
    }
}
