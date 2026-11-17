using Store.Core.Entities;

namespace Store.Core.Interfaces.Repositories;

/// <summary>Category persistence, including hierarchy delete and restore.</summary>
public interface ICategoryRepository : ICRUDRepository<Category>;
