using System;
using AutoMapper;
using Store.Core.DTO;
using Store.Core.Entities.Product;
using Store.Core.Interfaces;

namespace Store.API.Services;

public class ProductService(IProductRepository repository, IMapper mapper)
    : GenericService<Product, ProductDto, CreateProductRequest, UpdateProductRequest>(repository, mapper),
      IProductService
{

}
