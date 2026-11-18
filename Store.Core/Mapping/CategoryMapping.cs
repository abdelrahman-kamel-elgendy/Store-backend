using AutoMapper;
using Store.Core.DTOs.Category;
using Store.Core.Entities;

namespace Store.Core.Mapping;

public class CategoryMapping : Profile
{
    public CategoryMapping()
    {
        // 1. Entity -> Response
        CreateMap<Category, CategoryResponseDto>()
            // Map SubCategories to the IReadOnlyList property
            .ForMember(dest => dest.SubCategories, opt => opt.MapFrom(src => src.SubCategories));

        // 2. Request -> Entity
        CreateMap<CategoryRequestDto, Category>();
    }
}