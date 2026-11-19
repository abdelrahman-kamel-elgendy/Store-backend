using Store.Core.DTOs.Category;
using Store.Core.Entities;

namespace Store.Core.Interfaces.Repositories;

public interface ICategoryRepository : IBaseRepository<Category, CategoryFilter>
{
}
