using Microsoft.AspNetCore.Mvc;
using Store.API.Services;
using Store.Core.DTOs.Category;
using Store.Core.Entities;
using Store.Core.Interfaces.Services;

namespace Store.API.Controllers;

/// <summary> Manages product categories (create, read, update, soft-delete, restore)./// </summary>
[Route("api/[controller]")]
[ApiController]
public class CategoryController(ICategoryService service, IApiResponseBuilder responseBuilder)
    : BaseController<Category, CategoryResponseDto, CategoryRequestDto, CategoryFilter>(service, responseBuilder)
{
}
