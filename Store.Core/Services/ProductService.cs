using AutoMapper;
using Microsoft.AspNetCore.Http;
using Store.Core.DTOs;
using Store.Core.DTOs.Product;
using Store.Core.Entities;
using Store.Core.Exceptions;
using Store.Core.Interfaces.Repositories;
using Store.Core.Interfaces.Services;

namespace Store.Service.Services;

public class ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, IImageRepository imageRepository, IMapper mapper)
    : CRUDService<Product, ProductResponseDto, ProductRequestDto>(productRepository, mapper), IProductService
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;
    private readonly IImageRepository _imageRepository = imageRepository;
    private readonly IMapper _mapper = mapper;


    public override async Task<ProductResponseDto> CreateAsync(ProductRequestDto createProductRequest)
    {
        int categoryId = createProductRequest.CategoryId;
        string productName = createProductRequest.Name;
        IFormFileCollection? images = createProductRequest.Images;

        // category validation
        if (!await _categoryRepository.ExistsByIdAsync(categoryId))
            throw new NotFoundException($"Category with ID {categoryId} was not found.");

        // product validation
        if (await productRepository.ExistsByNameAsync(productName))
            throw new ConflictException($"Product with name {productName} already exists.");

        // product mapping
        var product = _mapper.Map<Product>(createProductRequest);

        // product creating
        await productRepository.AddAsync(product);

        // saving images
        if (images is not null)
            await _imageRepository.SaveImagesAsync(images, product.Id, productName);

        // response mapping
        return _mapper.Map<ProductResponseDto>(product);
    }

    public async Task<PagedResult<ProductResponseDto>> GetPageAsync(int? categoryId, int page, int pageSize, bool includeDeleted = false)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(pageSize, 100);

        var skip = (int)Math.Min((long)(page - 1) * pageSize, int.MaxValue);
        var totalItems = await productRepository.CountAsync(categoryId, includeDeleted);
        var entities = await productRepository.GetPageAsync(categoryId, skip, pageSize, includeDeleted);

        return new PagedResult<ProductResponseDto>(
            _mapper.Map<IReadOnlyList<ProductResponseDto>>(entities),
            page,
            pageSize,
            totalItems);
    }

    public override async Task<ProductResponseDto> UpdateAsync(int id, ProductRequestDto updateProductRequest)
    {
        int categoryId = updateProductRequest.CategoryId;
        string productName = updateProductRequest.Name;
        IFormFileCollection? images = updateProductRequest.Images;

        if (!await _categoryRepository.ExistsByIdAsync(categoryId))
            throw new NotFoundException($"Category with ID {categoryId} was not found.");

        var product = await productRepository.GetByIdAsync(id) ?? throw new NotFoundException($"Product with ID {id} was not found.");

        var existingProductWithName = await productRepository.GetByNameAsync(productName);
        if (existingProductWithName is not null && existingProductWithName.Id != id)
            if (!existingProductWithName.IsDeleted)
                throw new ConflictException($"Product with name '{productName}' already exists.");

        _mapper.Map(updateProductRequest, product);

        await _imageRepository.RemoveImagesAsync(id);
        if (images is not null)
            await _imageRepository.SaveImagesAsync(images, product.Id, productName);

        await productRepository.UpdateAsync(product);

        return _mapper.Map<ProductResponseDto>(product);
    }
}
