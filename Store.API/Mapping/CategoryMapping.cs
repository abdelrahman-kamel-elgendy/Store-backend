using AutoMapper;
using Store.Core.DTO.Category;
using Store.Core.Entities;

namespace Store.API.Mapping;

public class CategoryMapping : Profile
{
    public CategoryMapping()
    {
        CreateMap<Category, CategoryDto>()
            .ForCtorParam(nameof(CategoryDto.ParentCategoryId), options =>
                options.MapFrom(source => source.ParentCategoryId))
            .ForCtorParam(nameof(CategoryDto.SubCategories), options =>
                options.MapFrom(source => source.SubCategories));
        CreateMap<CreateCategoryRequest, Category>(MemberList.Source);
        CreateMap<UpdateCategoryRequest, Category>(MemberList.Source);

    }
}
