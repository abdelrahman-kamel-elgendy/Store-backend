using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Store.Core.Interfaces.Entities;
using Store.Core.Interfaces.Repositories;
using Store.Infrastructure.Data;

namespace Store.Infrastructure.Repositories;
/// <summary>Implements generic persistence operations using Entity Framework Core.</summary>
/// <typeparam name="T">The entity type tracked by this repository.</typeparam>
public class GenericRepository<T>(AppDbContext context) : IGenericRepository<T> where T : class
{
    private readonly AppDbContext _context = context;
    protected virtual string EntityName => typeof(T).Name;
    private static bool IsSoftDeletableType => typeof(ISoftDeletable).IsAssignableFrom(typeof(T));

    public async Task AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        if (entity is IAuditable auditable)
        {
            auditable.CreatedAt = DateTime.UtcNow;
            auditable.UpdatedAt = DateTime.UtcNow;
        }
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<T>> GetAllAsync() => await _context.Set<T>().AsNoTracking().ToListAsync();

    public async Task<IReadOnlyList<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
    {
        var query = _context.Set<T>().AsQueryable();

        foreach (var item in includes)
            query = query.Include(item);

        return await query.ToListAsync();
    }

    public virtual async Task<IReadOnlyList<T>> GetPageAsync(int skip, int take) =>
                await _context.Set<T>()
                    .AsNoTracking()
                    .OrderBy(entity => EF.Property<int>(entity, "Id"))
                    .Skip(skip)
                    .Take(take)
                    .ToListAsync();

    public virtual async Task<T?> GetByIdAsync(int entityId) => await GetEntityByIdAsync(entityId, false);

    public async Task<T?> GetByNameAsync(string entityName) => await _context.Set<T>().FirstOrDefaultAsync(entity => EF.Property<string>(entity, "Name") == entityName);

    public async Task<bool> ExistsByNameAsync(string name) => await _context.Set<T>().AnyAsync(entity => EF.Property<string>(entity, "Name") == name);

    public async Task<bool> ExistsByIdAsync(int id) => await _context.Set<T>().AnyAsync(entity => EF.Property<int>(entity, "Id") == id);

    public async Task UpdateAsync(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified;

        if (entity is IAuditable auditable)
            auditable.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int entityId)
    {
        var entity = await GetEntityByIdAsync(entityId, true);

        if (entity is ISoftDeletable softDeletable)
        {
            softDeletable.SoftDelete();
            if (entity is IAuditable auditable)
                auditable.UpdatedAt = DateTime.UtcNow;
        }
        else
            _context.Set<T>().Remove(entity);

        await _context.SaveChangesAsync();
    }

    public async Task<T> RestoreAsync(int entityId)
    {
        if (!IsSoftDeletableType)
            throw new InvalidOperationException($"{EntityName} does not support soft deletion.");

        var entity = await GetEntityByIdAsync(entityId, true);

        if (entity is not ISoftDeletable softDeletable)
            throw new InvalidOperationException($"{EntityName} does not support soft deletion.");

        if (!softDeletable.IsDeleted)
            throw new InvalidOperationException($"{EntityName} is alrady restored.");

        softDeletable.Restore();

        if (entity is IAuditable auditable)
            auditable.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<int> CountAsync() => await _context.Set<T>().CountAsync();

    private async Task<T> GetEntityByIdAsync(int entityId, bool includeDeleted)
    {
        var query = _context.Set<T>().AsQueryable();

        if (includeDeleted && IsSoftDeletableType)
            query = query.IgnoreQueryFilters();

        return await query.FirstOrDefaultAsync(entity => EF.Property<int>(entity, "Id") == entityId) ?? throw new InvalidOperationException($"{EntityName} with ID {entityId} was not found.");
    }
}
