using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Store.API.Errors;
using Store.API.Helper;
using Store.Core.DTO;
using Store.Core.Entities.Product;
using Store.Core.Interfaces;

namespace Store.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class CategoriesController(IUnitOfWork work, IMapper mapper) : BaseController(work, mapper)
    {
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<Category>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<Category>>>> Get()
        {
            var categories = await _work.CategoryRepository.GetAllAsync();

            return Ok(ApiResponses.Success(
                categories,
                categories.Count == 0 ? "No categories were found." : "Categories retrieved successfully."));
        }

        [HttpGet("id/{id}", Name = nameof(GetById))]
        [ProducesResponseType(typeof(ApiResponse<Category>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<Category>>> GetById(int id)
        {
            var category = await GetCategoryOrThrow(id);

            return Ok(ApiResponses.Success(
                category,
                $"Category with ID {id} retrieved successfully."));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Category>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<Category>>> Add([FromBody] CategoryDTO categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            await _work.CategoryRepository.AddAsync(category);

            return CreatedAtRoute(
                nameof(GetById),
                new { id = category.Id },
                ApiResponses.Success(
                    category,
                    "Category created successfully."));
        }

        [HttpPut("update")]
        [ProducesResponseType(typeof(ApiResponse<Category>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<Category>>> Update(UpdateCategoryDTO updateCategoryDto)
        {
            await GetCategoryOrThrow(updateCategoryDto.Id);
            var category = _mapper.Map<Category>(updateCategoryDto);
            await _work.CategoryRepository.UpdateAsync(category);

            return Ok(ApiResponses.Success(
                category,
                "Category updated successfully."));
        }

        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _work.CategoryRepository.DeleteAsync(id);
            }
            catch (ArgumentException)
            {
                throw new NotFoundException($"Category with ID {id} was not found.");
            }

            return NoContent();
        }

        private async Task<Category> GetCategoryOrThrow(int id)
        {
            try
            {
                return await _work.CategoryRepository.GetByIdAsync(id);
            }
            catch (ArgumentException)
            {
                throw new NotFoundException($"Category with ID {id} was not found.");
            }
        }
    }
}
