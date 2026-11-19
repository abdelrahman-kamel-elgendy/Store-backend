using Store.Core.DTOs.Category;
using Store.Core.Entities;

namespace Store.Core.Interfaces.Services;

public interface ICategoryService : IBaseService<CategoryResponseDto, CategoryRequestDto, CategoryFilter> { }
