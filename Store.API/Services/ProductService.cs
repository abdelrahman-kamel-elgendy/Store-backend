using AutoMapper;
using Store.API.Exceptions;
using Store.Core.DTO.Product;
using Store.Core.Entities;
using Store.Core.Interfaces.Repositories;
using Store.Core.Interfaces.Servics;

namespace Store.API.Services;

public class ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, IMapper mapper)
    : GenericService<Product, ProductDto, CreateProductRequest, UpdateProductRequest>(productRepository, mapper),
      IProductService
{
    public override async Task<ProductDto> CreateAsync(CreateProductRequest request)
    {
        int categoryId = request.CategoryId;
        string productName = request.Name;

        if (!await categoryRepository.ExistsByIdAsync(categoryId))
            throw new NotFoundException($"Category with ID {categoryId} was not found.");

        if (await productRepository.ExistsByNameAsync(productName))
            throw new ConflictException($"Product with name {productName} already exists.");

        return await base.CreateAsync(request);
    }

    public override async Task<ProductDto> UpdateAsync(int id, UpdateProductRequest request)
    {
        int categoryId = request.CategoryId;
        string productName = request.Name;

        if (!await categoryRepository.ExistsByIdAsync(categoryId))
            throw new NotFoundException($"Category with ID {categoryId} was not found.");

        var product = await productRepository.GetByNameAsync(productName);
        if (product is not null)
            if (!product.IsDeleted && product.Id != id)
                throw new ConflictException($"Category with name {productName} already exists.");

        return await base.UpdateAsync(id, request);
    }
}
