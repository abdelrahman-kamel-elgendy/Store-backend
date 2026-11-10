using Store.Core.DTO.Product;

namespace Store.Core.Interfaces.Servics;
public interface IProductService : IGenericService<ProductDto, CreateProductRequest, UpdateProductRequest>
{

}
