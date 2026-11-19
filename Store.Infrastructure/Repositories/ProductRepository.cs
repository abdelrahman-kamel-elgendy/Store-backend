using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Store.Core.DTOs.Product;
using Store.Core.Entities;
using Store.Core.Interfaces.Entities;
using Store.Core.Interfaces.Repositories;
using Store.Core.Interfaces.Services;
using Store.Infrastructure.Data;

namespace Store.Infrastructure.Repositories;

public class ProductRepository(AppDbContext context)
    : BaseRepository<Product, ProductFilter>(context), IProductRepository
{
    protected override IQueryable<Product> ApplyFilter(IQueryable<Product> query, ProductFilter filter)
    {
        if (filter.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == filter.CategoryId.Value);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim();
            query = query.Where(p => p.Name.Contains(term) || p.Description.Contains(term));
        }

        if (filter.MinPrice.HasValue)
            query = query.Where(p => p.NewPrice >= filter.MinPrice.Value);

        if (filter.MaxPrice.HasValue)
            query = query.Where(p => p.NewPrice <= filter.MaxPrice.Value);

        return query;
    }

    protected override IQueryable<Product> ApplyIncludes(IQueryable<Product> query) =>
        query.Include(p => p.Category).Include(p => p.Images);

    protected override IOrderedQueryable<Product> ApplySorting(IQueryable<Product> query, ProductFilter filter) => filter.Sort switch
    {
        ProductSort.Default => query.OrderBy(p => p.Id),
        ProductSort.PriceAsc => query.OrderBy(p => p.NewPrice).ThenBy(p => p.Id),
        ProductSort.PriceDesc => query.OrderByDescending(p => p.NewPrice).ThenBy(p => p.Id),
        ProductSort.NameAsc => query.OrderBy(p => p.Name).ThenBy(p => p.Id),
        ProductSort.NameDesc => query.OrderByDescending(p => p.Name).ThenBy(p => p.Id),
        ProductSort.Newest => query.OrderByDescending(p => p.CreatedAt).ThenBy(p => p.Id),
        _ => throw new ArgumentOutOfRangeException(nameof(filter), filter.Sort, "Unsupported sort order.")
    };
}
