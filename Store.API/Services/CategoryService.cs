using AutoMapper;
using Store.API.Exceptions;
using Store.Core.DTO;
using Store.Core.DTO.Category;
using Store.Core.Entities;
using Store.Core.Interfaces.Repositories;
using Store.Core.Interfaces.Servics;

namespace Store.API.Services;

public class CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
    : GenericService<Category, CategoryDto, CreateCategoryRequest, UpdateCategoryRequest>(categoryRepository, mapper),
      ICategoryService
{
    public override async Task<CategoryDto> CreateAsync(CreateCategoryRequest request)
    {
        string categoryName = request.Name;
        int? parentCategoryId = request.ParentCategoryId;

        if (await categoryRepository.ExistsByNameAsync(categoryName))
            throw new ConflictException($"Category with name {categoryName} already exists.");

        if (parentCategoryId is not null)
            if (!await categoryRepository.ExistsByIdAsync(parentCategoryId.Value))
                throw new NotFoundException($"Parent Category with ID '{parentCategoryId}' was not found.");

        return await base.CreateAsync(request);
    }

    public override async Task<PagedResult<CategoryDto>> GetAllAsync(int page, int pageSize)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(pageSize, 100);

        var categories = await categoryRepository.GetAllAsync();
        var categoriesByParentId = categories.ToLookup(category => category.ParentCategoryId);
        var rootCategories = categoriesByParentId[null].OrderBy(category => category.Id).ToArray();
        var skip = (int)Math.Min((long)(page - 1) * pageSize, int.MaxValue);

        return new PagedResult<CategoryDto>(
            rootCategories
                .Skip(skip)
                .Take(pageSize)
                .Select(category => BuildCategoryTree(
                    category,
                    categoriesByParentId,
                    new HashSet<int>()))
                .ToArray(),
            page,
            pageSize,
            rootCategories.Length);
    }

    public override async Task<CategoryDto> GetByIdAsync(int id)
    {
        var categories = await categoryRepository.GetAllAsync();
        var category = categories.FirstOrDefault(item => item.Id == id)
            ?? throw new NotFoundException($"Category with ID {id} was not found.");
        var categoriesByParentId = categories.ToLookup(item => item.ParentCategoryId);

        return BuildCategoryTree(category, categoriesByParentId, new HashSet<int>());
    }

    public override async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryRequest request)
    {
        string categoryName = request.Name;
        int? parentCategoryId = request.ParentCategoryId;

        if (parentCategoryId is not null)
            if (!await categoryRepository.ExistsByIdAsync(parentCategoryId.Value))
                throw new NotFoundException($"Parent Category with ID '{parentCategoryId}' was not found.");

        var category = await categoryRepository.GetByNameAsync(categoryName);

        if (category is not null)
        {
            if (!category.IsDeleted && category.Id != id)
                throw new ConflictException($"Category with name {categoryName} already exists.");

            if (category.Id == parentCategoryId)
                throw new BadRequestException($"The category cannot be its own parent.");
        }
        return await base.UpdateAsync(id, request);
    }

    private static CategoryDto BuildCategoryTree(
        Category category,
        ILookup<int?, Category> categoriesByParentId,
        HashSet<int> ancestorIds)
    {
        if (!ancestorIds.Add(category.Id))
            throw new InvalidOperationException($"A cycle was found in the category hierarchy at category ID {category.Id}.");

        var subCategories = categoriesByParentId[category.Id]
            .OrderBy(child => child.Id)
            .Select(child => BuildCategoryTree(child, categoriesByParentId, ancestorIds))
            .ToArray();

        ancestorIds.Remove(category.Id);

        return new CategoryDto(
            category.Id,
            category.Name,
            category.Description,
            category.ParentCategoryId,
            subCategories);
    }

}
