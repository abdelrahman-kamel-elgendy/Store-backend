using Store.Core.DTOs;
using Store.Core.DTOs.Product;
using Store.Core.Entities;

namespace Store.Core.Interfaces.Services;

public interface IProductService : IBaseService<ProductResponseDto, ProductRequestDto, ProductFilter> { }
