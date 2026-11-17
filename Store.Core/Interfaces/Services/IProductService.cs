using Store.Core.DTOs.Product;

namespace Store.Core.Interfaces.Services;

/// <summary>Product use cases.</summary>
public interface IProductService : ICRUDService<ProductResponseDto, ProductRequestDto>
{

}
