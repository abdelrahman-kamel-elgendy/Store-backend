using AutoMapper;
using Store.Core.DTOs;
using Store.Core.DTOs.Product;
using Store.Core.Entities;
using Store.Core.Exceptions;
using Store.Core.Interfaces.Repositories;
using Store.Core.Interfaces.Services;
using Store.Core.Services;

namespace Store.Service.Services;

public class ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, IImageRepository imageRepository, IMapper mapper)
    : BaseService<Product, ProductResponseDto, ProductRequestDto, ProductFilter>(productRepository, mapper), IProductService
{
    public async Task<PagedResult<ProductResponseDto>> GetAllAsync(ProductFilter filter, int page, int pageSize)
    {
        if (filter.CategoryId.HasValue && !await categoryRepository.ExistsByIdAsync(filter.CategoryId.Value))
            throw new NotFoundException($"Category with ID {filter.CategoryId} was not found.");

        return await base.GetAllPagedAsync(filter, page, pageSize);
    }

    public override async Task<ProductResponseDto> CreateAsync(ProductRequestDto request)
    {
        if (!await categoryRepository.ExistsByIdAsync(request.CategoryId))
            throw new NotFoundException($"Category with ID {request.CategoryId} was not found.");

        var product = _mapper.Map<Product>(request);
        await productRepository.AddAsync(product);

        if (request.Images is { Count: > 0 })
            await imageRepository.SaveImagesAsync(request.Images, product.Id, request.Name);

        return _mapper.Map<ProductResponseDto>(product);
    }

    public override async Task<ProductResponseDto> UpdateAsync(int id, ProductRequestDto request)
    {
        var product = await productRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Product with ID {id} was not found.");

        if (await categoryRepository.ExistsByIdAsync(request.CategoryId))
            throw new NotFoundException($"Category with ID {request.CategoryId} was not found.");

        var sameName = await productRepository.GetByNameAsync(request.Name);
        if (sameName is not null && sameName.Id != id)
            throw new ConflictException($"Product with name {request.Name} already exists.");

        _mapper.Map(request, product);

        await imageRepository.RemoveImagesAsync(id);
        await productRepository.UpdateAsync(product);

        if (request.Images is { Count: > 0 })
            await imageRepository.SaveImagesAsync(request.Images, id, request.Name);

        return _mapper.Map<ProductResponseDto>(product);
    }
}
