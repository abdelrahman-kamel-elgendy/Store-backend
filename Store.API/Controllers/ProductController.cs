using Microsoft.AspNetCore.Mvc;
using Store.API.Helper;
using Store.Core.DTO.Product;
using Store.Core.Entities;
using Store.Core.Interfaces.Servics;

namespace Store.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController(IProductService service) : BaseController<Product, ProductResponseDto, ProductRequestDto>(service)
{
}
