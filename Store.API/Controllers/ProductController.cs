using Microsoft.AspNetCore.Mvc;
using Store.Core.DTO.Product;
using Store.Core.Entities;
using Store.Core.Interfaces.Servics;

namespace Store.API.Controllers;

/// <summary> Manages product categories (create, read, update, soft-delete, restore)./// </summary>
[Route("api/[controller]")]
[ApiController]
public class ProductController(IProductService service) : BaseController<Product, ProductResponseDto, ProductRequestDto>(service)
{
}
