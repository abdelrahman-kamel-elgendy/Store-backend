using System;
using Store.Core.DTO;

namespace Store.Core.Interfaces;

public interface ICategoryService : IGenericService<CategoryDto, CreateCategoryRequest, UpdateCategoryRequest>
{

}
