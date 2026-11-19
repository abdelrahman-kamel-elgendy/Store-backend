using Microsoft.EntityFrameworkCore;
using Store.Core.DTOs;
using Store.Core.Entities;
using Store.Core.Interfaces.Repositories;
using Store.Infrastructure.Data;

namespace Store.Infrastructure.Repositories;

public abstract class BaseRepository<TEntity, TFilter>(AppDbContext context) : IBaseRepository<TEntity, TFilter>
    where TEntity : BaseEntity<int>
    where TFilter : IFilter
{
    protected readonly AppDbContext _context = context;
    protected virtual string EntityName => typeof(TEntity).Name;

    // hooks: override in a derived repository 
    protected virtual IQueryable<TEntity> ApplyFilter(IQueryable<TEntity> query, TFilter filter) => query;

    /// <summary>Adds Include(...) calls (used by page and single-row reads). Default: none.</summary>
    protected virtual IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> query) => query;

    /// <summary>Orders the query. Always end with a unique key so paging is stable.</summary>
    protected virtual IOrderedQueryable<TEntity> ApplySorting(IQueryable<TEntity> query, TFilter filter) =>
        query.OrderBy(entity => entity.Id);

    // query helpers 

    protected IQueryable<TEntity> Query(bool includeDeleted, bool tracked = true)
    {
        IQueryable<TEntity> query = tracked ? _context.Set<TEntity>() : _context.Set<TEntity>().AsNoTracking();
        return includeDeleted ? query.IgnoreQueryFilters() : query;
    }

    private IQueryable<TEntity> FilteredQuery(TFilter filter) =>
        ApplyFilter(Query(filter.IncludeDeleted, tracked: false), filter);

    // listing 

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(bool includeDeleted = false) =>
        await Query(includeDeleted, tracked: false).ToListAsync();

    public virtual async Task<IReadOnlyList<TEntity>> GetPageAsync(TFilter filter, int skip, int take) =>
        await ApplySorting(ApplyIncludes(FilteredQuery(filter)), filter)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

    // Same filters as the page, without includes or ordering.
    public virtual async Task<int> CountAsync(TFilter filter) =>
        await FilteredQuery(filter).CountAsync();

    // single-row reads 

    public virtual async Task<TEntity?> GetByIdAsync(int id, bool includeDeleted = false) =>
        await ApplyIncludes(Query(includeDeleted)).FirstOrDefaultAsync(entity => entity.Id == id);

    public virtual async Task<TEntity?> GetByNameAsync(string name, bool includeDeleted = false) =>
        await ApplyIncludes(Query(includeDeleted))
            .FirstOrDefaultAsync(entity => EF.Property<string>(entity, "Name") == name);

    public virtual async Task<bool> ExistsByIdAsync(int id, bool includeDeleted = false) =>
        await Query(includeDeleted, tracked: false).AnyAsync(entity => entity.Id == id);

    public virtual async Task<bool> ExistsByNameAsync(string name, bool includeDeleted = false) =>
        await Query(includeDeleted, tracked: false)
            .AnyAsync(entity => EF.Property<string>(entity, "Name") == name);

    // writes

    public virtual async Task AddAsync(TEntity entity)
    {
        await _context.Set<TEntity>().AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(TEntity entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(int id)
    {
        var entity = await Query(includeDeleted: false).FirstOrDefaultAsync(e => e.Id == id)
            ?? throw new InvalidOperationException($"{EntityName} with ID {id} was not found or may be deleted.");

        entity.SoftDelete();
        await _context.SaveChangesAsync();
    }

    public virtual async Task<TEntity> RestoreAsync(int id)
    {
        var entity = await Query(includeDeleted: true).FirstOrDefaultAsync(e => e.Id == id)
            ?? throw new InvalidOperationException($"{EntityName} with ID {id} was not found.");

        if (!entity.IsDeleted)
            throw new InvalidOperationException($"{EntityName} with ID {id} is already restored.");

        entity.Restore();
        await _context.SaveChangesAsync();
        return entity;
    }
}