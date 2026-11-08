using Store.Core.Entities.Product;
using Store.Core.Interfaces;
using Store.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Store.Infrastructure.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }
    }
}
