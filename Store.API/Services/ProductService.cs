using AutoMapper;
using Store.API.Exceptions;
using Store.Core.DTO.Product;
using Store.Core.Entities;
using Store.Core.Interfaces.Repositories;
using Store.Core.Interfaces.Servics;

namespace Store.API.Services;

public class ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, IImageRepository imageRepository, IMapper mapper) : GenericService<Product, ProductResponseDto, ProductRequestDto>(productRepository, mapper), IProductService
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly ICategoryRepository _categoryRepository = categoryRepository;
    private readonly IImageRepository _imageRepository = imageRepository;
    private readonly IMapper _mapper = mapper;

    public override async Task<ProductResponseDto> CreateAsync(ProductRequestDto createProductRequest)
    {
        int categoryId = createProductRequest.CategoryId;
        string productName = createProductRequest.Name;

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
        await _imageRepository.SaveImagesAsync(createProductRequest.Images, product.Id, productName);

        // response mapping
        return _mapper.Map<ProductResponseDto>(product);
    }

    public override async Task<ProductResponseDto> UpdateAsync(int id, ProductRequestDto updateProductRequest)
    {
        int categoryId = updateProductRequest.CategoryId;
        string productName = updateProductRequest.Name;

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
        await _imageRepository.SaveImagesAsync(updateProductRequest.Images, product.Id, productName);

        // product updating
        await _productRepository.UpdateAsync(product);

        // response mapping
        return _mapper.Map<ProductResponseDto>(product);
    }
}
