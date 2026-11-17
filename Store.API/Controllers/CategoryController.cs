using Microsoft.AspNetCore.Mvc;
using Store.Core.DTO.Category;
using Store.Core.Entities;
using Store.Core.Interfaces.Servics;

namespace Store.API.Controllers;

/// <summary> Manages product categories (create, read, update, soft-delete, restore)./// </summary>
[Route("api/[controller]")]
[ApiController]
public class CategoryController(ICategoryService service) : BaseController<Category, CategoryResponseDto, CategoryRequestDto>(service)
{
}
