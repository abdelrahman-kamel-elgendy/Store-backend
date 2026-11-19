using AutoMapper;
using Store.Core.DTOs;
using Store.Core.DTOs.Category;
using Store.Core.Entities;
using Store.Core.Exceptions;
using Store.Core.Interfaces.Repositories;
using Store.Core.Interfaces.Services;
using Store.Core.Services;

namespace Store.Service.Services;

public class CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
    : BaseService<Category, CategoryResponseDto, CategoryRequestDto, CategoryFilter>(categoryRepository, mapper), ICategoryService
{
    public override async Task<CategoryResponseDto> CreateAsync(CategoryRequestDto request)
    {
        if (await categoryRepository.ExistsByNameAsync(request.Name))
            throw new ConflictException($"Category with name {request.Name} already exists.");

        if (request.ParentCategoryId is { } parentId && !await categoryRepository.ExistsByIdAsync(parentId))
            throw new NotFoundException($"Parent Category with ID '{parentId}' was not found.");

        return await base.CreateAsync(request);
    }

    public override async Task<PagedResult<CategoryResponseDto>> GetAllPagedAsync(CategoryFilter filter, int page, int pageSize)
    {
        ValidatePaging(page, pageSize);

        var categories = await categoryRepository.GetAllAsync(filter.IncludeDeleted);
        var byParent = categories.ToLookup(c => c.ParentCategoryId);

        var roots = ApplySorting(ApplySearch(categories, byParent[null], filter.Search), filter.Sort).ToArray();
        var skip = (int)Math.Min((long)(page - 1) * pageSize, int.MaxValue);

        return new PagedResult<CategoryResponseDto>(
            roots.Skip(skip)
                 .Take(pageSize)
                 .Select(root => BuildCategoryTree(root, byParent, []))
                 .ToArray(),
            page,
            pageSize,
            roots.Length);
    }

    public override async Task<CategoryResponseDto> GetByIdAsync(int id, bool includeDeleted = false)
    {
        var categories = await categoryRepository.GetAllAsync(includeDeleted);
        var category = categories.FirstOrDefault(c => c.Id == id)
            ?? throw new NotFoundException($"Category with ID {id} was not found.");

        return BuildCategoryTree(category, categories.ToLookup(c => c.ParentCategoryId), []);
    }

    public override async Task<CategoryResponseDto> UpdateAsync(int id, CategoryRequestDto request)
    {
        var sameName = await categoryRepository.GetByNameAsync(request.Name);
        if (sameName is not null && sameName.Id != id)
            throw new ConflictException($"Category with name {request.Name} already exists.");

        if (request.ParentCategoryId is { } parentId)
            await EnsureValidParentAsync(id, parentId);

        await base.UpdateAsync(id, request);

        return await GetByIdAsync(id);
    }

    public override async Task<CategoryResponseDto> RestoreAsync(int id)
    {
        var category = await categoryRepository.GetByIdAsync(id, includeDeleted: true)
            ?? throw new NotFoundException($"Category with ID {id} was not found.");

        if (!category.IsDeleted)
            throw new ConflictException($"Category with ID {id} is not deleted.");

        if (category.ParentCategoryId is { } parentId)
        {
            var parent = await categoryRepository.GetByIdAsync(parentId, includeDeleted: true);
            if (parent is { IsDeleted: true })
                throw new BusinessRuleException($"Restore the parent category (ID {parentId}) first.");
        }

        await categoryRepository.RestoreAsync(id);

        return await GetByIdAsync(id);
    }


    private async Task EnsureValidParentAsync(int id, int parentId)
    {
        if (parentId == id)
            throw new BadRequestException("The category cannot be its own parent.");

        var all = await categoryRepository.GetAllAsync();
        var byId = all.ToDictionary(c => c.Id);

        if (!byId.ContainsKey(parentId))
            throw new NotFoundException($"Parent Category with ID '{parentId}' was not found.");

        var visited = new HashSet<int>();
        for (int? current = parentId; current is { } currentId && visited.Add(currentId);)
        {
            if (currentId == id)
                throw new BadRequestException("A category cannot be moved under one of its own subcategories.");

            current = byId.TryGetValue(currentId, out var node) ? node.ParentCategoryId : null;
        }
    }

    private static IEnumerable<Category> ApplySearch(
        IReadOnlyList<Category> all, IEnumerable<Category> roots, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return roots;

        var term = search.Trim();
        var byId = all.ToDictionary(c => c.Id);
        var visibleIds = new HashSet<int>();

        foreach (var match in all.Where(c => c.Name.Contains(term, StringComparison.OrdinalIgnoreCase)))
        {
            Category? current = match;
            while (current is not null && visibleIds.Add(current.Id))
                current = current.ParentCategoryId is { } pid && byId.TryGetValue(pid, out var parent) ? parent : null;
        }

        return roots.Where(r => visibleIds.Contains(r.Id));
    }

    private static IEnumerable<Category> ApplySorting(IEnumerable<Category> roots, CategorySort sort) =>
        sort switch
        {
            CategorySort.Default => roots.OrderBy(c => c.Id),
            CategorySort.NameAsc => roots.OrderBy(c => c.Name, StringComparer.OrdinalIgnoreCase).ThenBy(c => c.Id),
            CategorySort.NameDesc => roots.OrderByDescending(c => c.Name, StringComparer.OrdinalIgnoreCase).ThenBy(c => c.Id),
            CategorySort.Newest => roots.OrderByDescending(c => c.CreatedAt).ThenBy(c => c.Id),
            _ => throw new ArgumentOutOfRangeException(nameof(sort), sort, "Unsupported sort order.")
        };

    private static CategoryResponseDto BuildCategoryTree(
        Category category, ILookup<int?, Category> byParent, HashSet<int> ancestorIds)
    {
        if (!ancestorIds.Add(category.Id))
            throw new InvalidOperationException($"A cycle was found in the category hierarchy at category ID {category.Id}.");

        var subCategories = byParent[category.Id]
            .OrderBy(child => child.Id)
            .Select(child => BuildCategoryTree(child, byParent, ancestorIds))
            .ToArray();

        ancestorIds.Remove(category.Id);

        return new CategoryResponseDto(
            category.Id, category.Name, category.Description, category.ParentCategoryId,
            subCategories, category.IsDeleted, category.CreatedAt, category.UpdatedAt, category.DeletedAt);
    }

}
