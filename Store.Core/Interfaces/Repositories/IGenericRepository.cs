namespace Store.Core.Interfaces.Repositories;
/// <summary>Defines asynchronous persistence operations for an entity type.</summary>
/// <typeparam name="T">The entity type.</typeparam>
public interface IGenericRepository<T> where T : class
{
    Task AddAsync(T entity);
    Task<IReadOnlyList<T>> GetAllAsync(bool includeDeleted = false);
    Task<IReadOnlyList<T>> GetPageAsync(int skip, int take, bool includeDeleted = false);
    Task<T?> GetByIdAsync(int entityId, bool includeDeleted = false);
    Task<T?> GetByNameAsync(string entityName);
    Task<bool> ExistsByNameAsync(string name);
    Task<bool> ExistsByIdAsync(int id);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int entityId);
    Task<T> RestoreAsync(int entityId);
    Task<int> CountAsync(bool includeDeleted = false);
}
