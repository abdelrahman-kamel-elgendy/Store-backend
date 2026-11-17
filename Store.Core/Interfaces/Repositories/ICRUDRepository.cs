namespace Store.Core.Interfaces.Repositories;

/// <summary>Persistence operations for one entity type.</summary>
/// <typeparam name="T">The entity type.</typeparam>
public interface ICRUDRepository<T> where T : class
{
    /// <summary>Inserts an entity and saves it.</summary>
    /// <param name="entity">The entity to insert.</param>
    Task AddAsync(T entity);

    /// <summary>Returns every matching entity without paging.</summary>
    /// <param name="includeDeleted">When true, soft-deleted rows are included.</param>
    Task<IReadOnlyList<T>> GetAllAsync(bool includeDeleted = false);

    /// <summary>Returns one page of entities.</summary>
    /// <param name="skip">Number of matching rows to skip.</param>
    /// <param name="take">Number of rows to return.</param>
    /// <param name="includeDeleted">When true, soft-deleted rows are included.</param>
    Task<IReadOnlyList<T>> GetPageAsync(int skip, int take, bool includeDeleted = false);

    /// <summary>Finds an entity by id.</summary>
    /// <param name="entityId">Entity id.</param>
    /// <param name="includeDeleted">When true, a soft-deleted row can be returned.</param>
    /// <returns>The entity, or null when it does not exist.</returns>
    Task<T?> GetByIdAsync(int entityId, bool includeDeleted = false);

    /// <summary>Finds a non-deleted entity by name.</summary>
    /// <param name="entityName">Name to match.</param>
    Task<T?> GetByNameAsync(string entityName);

    /// <summary>Returns true when a non-deleted entity has this name.</summary>
    /// <param name="name">Name to match.</param>
    Task<bool> ExistsByNameAsync(string name);

    /// <summary>Returns true when a non-deleted entity has this id.</summary>
    /// <param name="id">Entity id.</param>
    Task<bool> ExistsByIdAsync(int id);

    /// <summary>Saves changes to an entity.</summary>
    /// <param name="entity">The entity to update.</param>
    Task UpdateAsync(T entity);

    /// <summary>Soft-deletes an entity.</summary>
    /// <param name="entityId">Entity id.</param>
    Task DeleteAsync(int entityId);

    /// <summary>Restores a soft-deleted entity.</summary>
    /// <param name="entityId">Entity id.</param>
    Task<T> RestoreAsync(int entityId);

    /// <summary>Counts entities that match the filter.</summary>
    /// <param name="includeDeleted">When true, soft-deleted rows are counted.</param>
    Task<int> CountAsync(bool includeDeleted = false);
}
