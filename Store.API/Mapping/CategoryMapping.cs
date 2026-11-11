using AutoMapper;
using Store.Core.DTO.Category;
using Store.Core.Entities;

namespace Store.API.Mapping;

public class CategoryMapping : Profile
{
    public CategoryMapping()
    {
        // Entity -> Response
        // Constructor params (Id, Name, Description, ParentCategoryId, SubCategories)
        // match the entity's property names, so AutoMapper binds them by convention.
        // The self-referencing SubCategories collection is mapped recursively.
        CreateMap<Category, CategoryResponseDto>();

        // Request -> Entity (used for both create and update via Map(dto, entity))
        CreateMap<CategoryRequestDto, Category>(MemberList.Source)
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ParentCategory, opt => opt.Ignore())
            .ForMember(dest => dest.SubCategories, opt => opt.Ignore())
            .ForMember(dest => dest.Products, opt => opt.Ignore());
    }
}
