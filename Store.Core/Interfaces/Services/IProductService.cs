using System;
using Store.Core.DTO;

namespace Store.Core.Interfaces;

public interface IProductService : IGenericService<ProductDto, CreateProductRequest, UpdateProductRequest>
{

}
