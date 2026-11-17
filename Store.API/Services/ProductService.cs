using AutoMapper;
using Store.API.Exceptions;
using Store.Core.DTOs;
using Store.Core.DTOs.Product;
using Store.Core.Entities;
using Store.Core.Interfaces.Repositories;
using Store.Core.Interfaces.Services;

namespace Store.API.Services;

public class ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, IImageRepository imageRepository, IMapper mapper) : CRUDService<Product, ProductResponseDto, ProductRequestDto>(productRepository, mapper), IProductService
{
    private readonly IProductRepository _productRepository = productRepository;
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
        if (await _productRepository.ExistsByNameAsync(productName))
            throw new ConflictException($"Product with name {productName} already exists.");

        // product mapping
        var product = _mapper.Map<Product>(createProductRequest);

        // product creating
        await _productRepository.AddAsync(product);

        // saving images
        if (images is not null)
            await _imageRepository.SaveImagesAsync(images, product.Id, productName);

        // response mapping
        return _mapper.Map<ProductResponseDto>(product);
    }

    public async Task<PagedResult<ProductResponseDto>> GetByCategoryIdAsync(int categoryId, int page, int pageSize, bool includeDeleted = false)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(pageSize, 100);

        var skip = (int)Math.Min((long)(page - 1) * pageSize, int.MaxValue);
        var totalItems = await _productRepository.CountByCategoryAsync(categoryId, includeDeleted);
        var entities = await _productRepository.GetByCategoryIdAsync(categoryId, skip, pageSize, includeDeleted);

        return new PagedResult<ProductResponseDto>(
            _mapper.Map<IReadOnlyList<ProductResponseDto>>(entities),
            page,
            pageSize,
            totalItems);
    }

    public async Task<PagedResult<ProductResponseDto>> GetByCategoryNameAsync(string categoryName, int page, int pageSize, bool includeDeleted = false)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(pageSize, 100);

        var skip = (int)Math.Min((long)(page - 1) * pageSize, int.MaxValue);
        var totalItems = await _productRepository.CountByCategoryAsync(categoryName, includeDeleted);
        var entities = await _productRepository.GetByCategoryNameAsync(categoryName, skip, pageSize, includeDeleted);

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

        // catgory validation
        if (!await _categoryRepository.ExistsByIdAsync(categoryId))
            throw new NotFoundException($"Category with ID {categoryId} was not found.");

        // product validation
        if (!await _productRepository.ExistsByIdAsync(id))
            throw new NotFoundException($"Product with ID {categoryId} was not found.");

        var existingProduct = await _productRepository.GetByNameAsync(productName);
        if (existingProduct is not null)
            if (!existingProduct.IsDeleted && existingProduct.Id != id)
                throw new ConflictException($"Category with name {productName} already exists.");

        // product mapping
        var product = _mapper.Map<Product>(updateProductRequest);

        // remove product old imags
        await _imageRepository.RemoveImagesAsync(id);

        // saving product new imags
        if (images is not null)
            await _imageRepository.SaveImagesAsync(images, product.Id, productName);

        // product updating
        await _productRepository.UpdateAsync(product);

        // response mapping
        return _mapper.Map<ProductResponseDto>(product);
    }
}
