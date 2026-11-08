using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Store.Core.Exceptions;
using Store.Core.Interfaces;
using Store.Infrastructure.Data;

namespace Store.Infrastructure.Repositories
{
    /// <summary>Implements generic persistence operations using Entity Framework Core.</summary>
    /// <typeparam name="T">The entity type tracked by this repository.</typeparam>
    public class GenericRepository<T>(AppDbContext context) : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context = context;

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int entityId)
        {
            var entity = await _context.Set<T>().FindAsync([entityId]) ?? throw new NotFoundException($"Entity with ID {entityId} was not found.");
            _context.Set<T>().Remove(entity);
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

        public async Task<IReadOnlyList<T>> GetPageAsync(int skip, int take) =>
            await _context.Set<T>()
                .AsNoTracking()
                .OrderBy(entity => EF.Property<int>(entity, "Id"))
                .Skip(skip)
                .Take(take)
                .ToListAsync();

        public async Task<int> CountAsync() => await _context.Set<T>().CountAsync();

        public async Task<T> GetByIdAsync(int entityId) => await _context.Set<T>().FindAsync([entityId]) ?? throw new NotFoundException($"Entity with ID {entityId} was not found.");

        public async Task UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified; // Mark the entity as modified
            await _context.SaveChangesAsync();
        }
    }
}
