using Microsoft.AspNetCore.Mvc;
using Store.Core.DTO;
using Store.Core.Interfaces;

namespace Store.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(ICategoryService service) : BaseController<CategoryDto, CreateCategoryRequest, UpdateCategoryRequest>(service)
    {
    }
}
