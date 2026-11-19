using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Store.Core.Entities;
using Store.Core.Interfaces.Repositories;
using Store.Core.Interfaces.Services;
using Store.Infrastructure.Data;

namespace Store.Infrastructure.Repositories;

public class ProductRepository(AppDbContext context) : CRUDRepository<Product>(context), IProductRepository
{

    public async Task<IReadOnlyList<Product>> GetPageAsync(int? categoryId, int skip, int take, bool includeDeleted = false)
    {
        var query = _context.Products.AsNoTracking();

        if (includeDeleted)
            query = query.IgnoreQueryFilters();

        if (categoryId.HasValue)
            query = query.Where(product => product.CategoryId == categoryId.Value);

        return await query
            .Include(product => product.Category)
            .Include(product => product.Images) 
            .OrderBy(product => product.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public override async Task<Product?> GetByIdAsync(int productId, bool includeDeleted = false)
    {
        var query = _context.Products.AsNoTrackingWithIdentityResolution();

        if (includeDeleted && IsSoftDeletableType)
            query = query.IgnoreQueryFilters();

        return await query
            .Include(product => product.Category)
            .Include(product => product.Images)
            .FirstOrDefaultAsync(p => p.Id == productId);
    }

    public async Task<int> CountAsync(int? categoryId, bool includeDeleted = false)
    {
        var query = _context.Set<Product>().AsQueryable();

        if (includeDeleted)
            query = query.IgnoreQueryFilters();

        if (categoryId.HasValue)
            query = query.Where(product => product.CategoryId == categoryId.Value);

        return await query.CountAsync();
    }
}
