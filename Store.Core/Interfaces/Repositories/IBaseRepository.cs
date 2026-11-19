using System;

namespace Store.Core.Interfaces.Repositories;

/// <summary>Defines asynchronous persistence operations for an entity type.</summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TFilter">The criteria accepted when listing entities.</typeparam>
public interface IBaseRepository<TEntity, TFilter>
{
    Task AddAsync(TEntity entity);
    Task<IReadOnlyList<TEntity>> GetAllAsync(bool includeDeleted = false);
    Task<IReadOnlyList<TEntity>> GetPageAsync(TFilter filter, int skip, int take);
    Task<int> CountAsync(TFilter filter);
    Task<TEntity?> GetByIdAsync(int id, bool includeDeleted = false);
    Task<TEntity?> GetByNameAsync(string name, bool includeDeleted = false);
    Task<bool> ExistsByIdAsync(int id, bool includeDeleted = false);
    Task<bool> ExistsByNameAsync(string name, bool includeDeleted = false);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(int id);
    Task<TEntity> RestoreAsync(int id);
}

