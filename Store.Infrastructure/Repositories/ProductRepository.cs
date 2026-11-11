using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Store.Core.Entities;
using Store.Core.Interfaces.Repositories;
using Store.Core.Interfaces.Services;
using Store.Infrastructure.Data;

namespace Store.Infrastructure.Repositories;

public class ProductRepository(AppDbContext context) : GenericRepository<Product>(context), IProductRepository
{

    public override async Task<IReadOnlyList<Product>> GetPageAsync(int skip, int take, bool includeDeleted = false)
    {
        var query = _context.Products.AsNoTracking();
        if (includeDeleted)
            query = query.IgnoreQueryFilters();

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
        var query = _context.Products.AsQueryable();
        if (includeDeleted)
            query = query.IgnoreQueryFilters();

        return await query
            .Include(product => product.Category)
            .Include(product => product.Images)
            .FirstOrDefaultAsync(product => product.Id == productId);
    }

    public override async Task<Product?> GetByNameAsync(string productName) =>
        await _context.Products
            .Include(product => product.Category)
            .Include(product => product.Images)
            .FirstOrDefaultAsync(product => product.Name == productName);


}
