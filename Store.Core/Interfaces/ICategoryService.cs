using System;
using Store.Core.DTO;
using Store.Core.DTO.Category;

namespace Store.Core.Interfaces;

public interface ICategoryService : IGenericService<CategoryDto, CreateCategoryRequest, UpdateCategoryRequest>
{

}
