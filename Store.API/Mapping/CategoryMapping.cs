using System;
using AutoMapper;
using Store.Core.DTO;
using Store.Core.Entities.Product;

namespace Store.API.Mapping;

public class CategoryMapping : Profile
{
    public CategoryMapping()
    {
        CreateMap<CategoryDTO, Category>().ReverseMap();
        CreateMap<UpdateCategoryDTO, Category>().ReverseMap();
    }
}
