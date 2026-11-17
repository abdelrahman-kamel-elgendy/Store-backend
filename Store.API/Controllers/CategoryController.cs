using Microsoft.AspNetCore.Mvc;
using Store.Core.DTOs.Category;
using Store.Core.Entities;
using Store.Core.Interfaces.Services;

namespace Store.API.Controllers;

/// <summary> Manages product categories (create, read, update, soft-delete, restore)./// </summary>
[Route("api/[controller]")]
[ApiController]
public class CategoryController(ICategoryService service) : CRUDController<Category, CategoryResponseDto, CategoryRequestDto>(service)
{
}
