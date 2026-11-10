using Store.Core.Entities;
using Store.Core.Interfaces.Repositories;
using Store.Infrastructure.Data;

namespace Store.Infrastructure.Repositories;

public class CategoryRepository(AppDbContext context) : GenericRepository<Category>(context), ICategoryRepository
{
}
