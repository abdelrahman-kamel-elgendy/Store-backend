using Microsoft.EntityFrameworkCore;
using Store.Core.Entities;
using Store.Core.Interfaces.Repositories;
using Store.Infrastructure.Data;

namespace Store.Infrastructure.Repositories;

public class CategoryRepository(AppDbContext context) : GenericRepository<Category>(context), ICategoryRepository
{
    public override async Task DeleteAsync(int categoryId)
    {
        // Tracked query so SaveChanges persists the changes.
        var categories = await _context.Categories.ToListAsync();   // filter already hides deleted rows

        var category = categories.FirstOrDefault(c => c.Id == categoryId && !c.IsDeleted) ?? throw new InvalidOperationException($"Catgory with ID {categoryId} not found.");

        var byParent = categories.ToLookup(c => c.ParentCategoryId);
        var toDelete = GetSubCategories(category, byParent, []);   // all descendants
        toDelete.Add(category);

        var now = DateTime.UtcNow;
        foreach (var item in toDelete)
        {
            item.SoftDelete();
            item.UpdatedAt = now;
        }

        await _context.SaveChangesAsync();   // one save, one transaction: all or nothing
    }

    public override async Task<Category> RestoreAsync(int categoryId)
    {
        // IgnoreQueryFilters so soft-deleted rows are loaded. Tracked query so SaveChanges persists the changes.
        var categories = await _context.Categories
            .IgnoreQueryFilters()
            .ToListAsync();

        var category = categories.FirstOrDefault(c => c.Id == categoryId && c.IsDeleted)
            ?? throw new InvalidOperationException($"Deleted {EntityName} with ID {categoryId} was not found.");

        // Don't bring a subcategory back under a parent that is still deleted.
        if (category.ParentCategoryId is { } parentId &&
            categories.Any(c => c.Id == parentId && c.IsDeleted))
            throw new InvalidOperationException($"Restore the parent category (ID {parentId}) first.");

        var byParent = categories.ToLookup(c => c.ParentCategoryId);

        var toRestore = GetSubCategories(category, byParent, [])
            .Where(c => c.IsDeleted)
            .ToList();
        toRestore.Add(category);

        var now = DateTime.UtcNow;
        foreach (var item in toRestore)
        {
            item.Restore();
            item.UpdatedAt = now;
        }

        await _context.SaveChangesAsync();   // one save, one transaction: all or nothing
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
