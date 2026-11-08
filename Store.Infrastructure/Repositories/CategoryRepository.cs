using System;
using Store.Core.Entities.Product;
using Store.Core.Interfaces;
using Store.Infrastructure.Data;

namespace Store.Infrastructure.Repositories;

public class CategoryRepository(AppDbContext context) : GenericRepository<Category>(context), Core.Interfaces.ICategoryRepository
{
}
