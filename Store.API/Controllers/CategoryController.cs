using Microsoft.AspNetCore.Mvc;
using Store.Core.DTO.Category;
using Store.Core.Entities;
using Store.Core.Interfaces.Servics;

namespace Store.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController(ICategoryService service) : BaseController<Category, CategoryResponseDto, CategoryRequestDto>(service)
{
}
