using Microsoft.EntityFrameworkCore;
using Store.Core.DTOs.Category;
using Store.Core.Entities;
using Store.Core.Interfaces.Repositories;
using Store.Infrastructure.Data;

namespace Store.Infrastructure.Repositories;

/// <summary>Persists categories, including hierarchy-aware delete and restore.</summary>
public class CategoryRepository(AppDbContext context)
    : BaseRepository<Category, CategoryFilter>(context), ICategoryRepository
{
    protected override IQueryable<Category> ApplyFilter(IQueryable<Category> query, CategoryFilter filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim();
            query = query.Where(c => c.Name.Contains(term));
        }

        return query;
    }

    protected override IOrderedQueryable<Category> ApplySorting(IQueryable<Category> query, CategoryFilter filter) =>
        filter.Sort switch
        {
            CategorySort.Default => query.OrderBy(c => c.Id),
            CategorySort.NameAsc => query.OrderBy(c => c.Name).ThenBy(c => c.Id),
            CategorySort.NameDesc => query.OrderByDescending(c => c.Name).ThenBy(c => c.Id),
            CategorySort.Newest => query.OrderByDescending(c => c.CreatedAt).ThenBy(c => c.Id),
            _ => throw new ArgumentOutOfRangeException(nameof(filter), filter.Sort, "Unsupported sort order.")
        };


    public override async Task DeleteAsync(int categoryId)
    {
        var categories = await Query(includeDeleted: false).ToListAsync();

        var category = categories.FirstOrDefault(c => c.Id == categoryId)
            ?? throw new InvalidOperationException($"{EntityName} with ID {categoryId} was not found.");

        var byParent = categories.ToLookup(c => c.ParentCategoryId);
        var toDelete = GetSubCategories(category, byParent, []);
        toDelete.Add(category);

        foreach (var item in toDelete)
            item.SoftDelete();

        await _context.SaveChangesAsync();
    }

    public override async Task<Category> RestoreAsync(int categoryId)
    {
        var categories = await Query(includeDeleted: true).ToListAsync();

        var category = categories.FirstOrDefault(c => c.Id == categoryId && c.IsDeleted)
            ?? throw new InvalidOperationException($"Deleted {EntityName} with ID {categoryId} was not found.");

        if (category.ParentCategoryId is { } parentId && categories.Any(c => c.Id == parentId && c.IsDeleted))
            throw new InvalidOperationException($"Restore the parent category (ID {parentId}) first.");

        var byParent = categories.ToLookup(c => c.ParentCategoryId);
        var toRestore = GetSubCategories(category, byParent, [])
            .Where(c => c.IsDeleted)
            .ToList();
        toRestore.Add(category);

        foreach (var item in toRestore)
            item.Restore();

        await _context.SaveChangesAsync();
        return category;
    }

    private static List<Category> GetSubCategories(Category category, ILookup<int?, Category> categoriesByParentId, HashSet<int> visitedIds)
    {
        var result = new List<Category>();
        var queue = new Queue<Category>();

        visitedIds.Add(category.Id);
        queue.Enqueue(category);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            foreach (var child in categoriesByParentId[current.Id].OrderBy(c => c.Id))
            {
                if (!visitedIds.Add(child.Id))
                    throw new InvalidOperationException($"A cycle was found in the category hierarchy at category ID {child.Id}.");

                result.Add(child);
                queue.Enqueue(child);
            }
        }

        return result;
    }
}