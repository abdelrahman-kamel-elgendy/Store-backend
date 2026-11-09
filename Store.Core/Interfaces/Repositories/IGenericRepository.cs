using System;
using System.Linq.Expressions;

namespace Store.Core.Interfaces
{
    /// <summary>Defines asynchronous persistence operations for an entity type.</summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public interface IGenericRepository<T> where T : class
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<IReadOnlyList<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
        Task<IReadOnlyList<T>> GetPageAsync(int skip, int take);
        Task<T> GetByIdAsync(int entityId);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int entityId);
        Task<int> CountAsync();
    }
}
