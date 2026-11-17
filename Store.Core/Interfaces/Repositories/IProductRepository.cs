using Store.Core.Entities;

namespace Store.Core.Interfaces.Repositories;

/// <summary>Product persistence. Reads include the category and images.</summary>
public interface IProductRepository : IGenericRepository<Product>;
