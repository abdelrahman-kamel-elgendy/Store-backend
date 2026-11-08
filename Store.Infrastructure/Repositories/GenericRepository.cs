using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Store.Core.Entities;
using Store.Core.Interfaces;
using Store.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Store.Infrastructure.Repositories
{
    public class GenericRepository <T>: IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        public GenericRepository(AppDbContext context) => _context = context;
        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int entityId)
        {
            var entity = await _context.Set<T>().FindAsync(entityId) ?? throw new ArgumentException($"Entity with ID {entityId} not found.");
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllAsync() => await _context.Set<T>().AsNoTracking().ToListAsync();

        public async Task<IReadOnlyList<T>> GetAllAsync(params System.Linq.Expressions.Expression<Func<T, object>>[] includes)
        {
            var query = _context.Set<T>().AsQueryable();
            foreach (var item in includes)
                query = query.Include(item);

            return await query.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int entityId) => await _context.Set<T>().FindAsync(entityId) ?? throw new ArgumentException($"Entity with ID {entityId} not found.");

        public async Task<T> GetByIdAsync(int entityId, params System.Linq.Expressions.Expression<Func<T, object>>[] includes)
        {
            var query = _context.Set<T>().AsQueryable();
            foreach (var item in includes)
                query = query.Include(item);

            return await query.FirstOrDefaultAsync(entity => EF.Property<int>(entity, "Id") == entityId) ?? throw new ArgumentException($"Entity with ID {entityId} not found.");
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified; // Mark the entity as modified
            await _context.SaveChangesAsync();
        }
    }
}
