using Store.Core.DTOs.Product;
using Store.Core.Entities;

namespace Store.Core.Interfaces.Repositories;

public interface IProductRepository : IBaseRepository<Product, ProductFilter>
{
}