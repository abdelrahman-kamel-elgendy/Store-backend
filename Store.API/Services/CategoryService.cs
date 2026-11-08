using System;
using AutoMapper;
using Store.Core.DTO.Category;
using Store.Core.Entities.Product;
using Store.Core.Interfaces;

namespace Store.API.Services;

public class CategoryService(ICategoryRepository repository, IMapper mapper)
    : GenericService<Category, CategoryDto, CreateCategoryRequest, UpdateCategoryRequest>(repository, mapper),
      ICategoryService
{

}
