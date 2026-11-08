using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.API.Services;
using Store.Core.DTO.Category;
using Store.Core.Entities.Product;
using Store.Core.Interfaces;

namespace Store.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(ICategoryService service) : BaseController<CategoryDto, CreateCategoryRequest, UpdateCategoryRequest>(service)
    {
    }
}
