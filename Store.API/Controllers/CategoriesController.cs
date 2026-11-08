using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Store.Core.DTO;
using Store.Core.Entities.Product;
using Store.Core.Interfaces;

namespace Store.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController(IUnitOfWork work, IMapper mapper) : BaseController(work, mapper)
    {
        [HttpGet()]
        public async Task<IActionResult> get()
        {
            try
            {
                var categories = await _work.CategoryRepository.GetAllAsync();
                if (categories is null)
                    return BadRequest();

                return Ok(categories);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("id/{id}")]
        public async Task<IActionResult> getById(int id)
        {
            try
            {
                var category = await _work.CategoryRepository.GetByIdAsync(id);
                if (category is null)
                    return BadRequest();

                return Ok(category);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost()]
        public async Task<IActionResult> add(CategoryDTO categoryDto)
        {
            try
            {
                var category = _mapper.Map<Category>(categoryDto);
                await _work.CategoryRepository.AddAsync(category);

                return Ok(new { Message = "Category has been added successfully" });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> update(UpdateCategoryDTO updateCategoryDto)
        {
            try
            {
                var category = _mapper.Map<Category>(updateCategoryDto);
                await _work.CategoryRepository.UpdateAsync(category);

                return Ok(new { Message = "Category has been updated successfully" });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> delete(int id)
        {
            try
            {
                await _work.CategoryRepository.DeleteAsync(id);
                return Ok(new { Message = "Category has been deleted successfully" });
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }
    }
}
