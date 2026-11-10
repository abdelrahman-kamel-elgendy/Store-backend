using Microsoft.EntityFrameworkCore;
using Store.Core.Entities;
using Store.Core.Interfaces.Repositories;
using Store.Infrastructure.Data;

namespace Store.Infrastructure.Repositories;

public class ProductRepository(AppDbContext context) : GenericRepository<Product>(context), IProductRepository
{
    private readonly AppDbContext _context = context;

    public override async Task<IReadOnlyList<Product>> GetPageAsync(int skip, int take) =>
        await _context.Products
            .AsNoTracking()
            .Include(product => product.Category)
            .Include(product => product.Photos)
            .OrderBy(product => product.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

    public override async Task<Product?> GetByIdAsync(int entityId) =>
        await _context.Products
            .Include(product => product.Category)
            .Include(product => product.Photos)
            .FirstOrDefaultAsync(product => product.Id == entityId);
}
