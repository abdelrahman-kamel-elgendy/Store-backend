using Microsoft.AspNetCore.Mvc;
using Store.API.DTO;
using Store.API.Helper;
using Store.API.Services;
using Store.Core.DTOs.Product;
using Store.Core.Entities;
using Store.Core.Interfaces.Services;

namespace Store.API.Controllers;

/// <summary>Manages products (create, read, update, soft-delete, restore).</summary>
[Route("api/[controller]")]
[ApiController]
public class ProductController(IProductService service, IApiResponseBuilder responseBuilder)
    : BaseController<Product, ProductResponseDto, ProductRequestDto, ProductFilter>(service, responseBuilder)
{
    // Products carry IFormFileCollection, so they are bound from multipart/form-data, not JSON.
    [HttpPost]
    public override Task<ActionResult<ApiResponse<ProductResponseDto>>> Create([FromForm] ProductRequestDto request) =>
        base.Create(request);

    [HttpPut("{id:int}")]
    public override Task<ActionResult<ApiResponse<ProductResponseDto>>> Update(int id, [FromForm] ProductRequestDto request) =>
        base.Update(id, request);
}