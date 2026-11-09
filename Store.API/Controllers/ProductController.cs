using Microsoft.AspNetCore.Mvc;
using Store.Core.DTO;
using Store.Core.Interfaces;

namespace Store.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProductService service) : BaseController<ProductDto, CreateProductRequest, UpdateProductRequest>(service)
    {
    }
}
