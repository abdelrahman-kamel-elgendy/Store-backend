using System;
using Store.Core.Entities.Product;

namespace Store.Core.Interfaces;

public interface IProductService : IGenericService<ProductDto, CreateProductRequest, UpdateProductRequest>
{

}
