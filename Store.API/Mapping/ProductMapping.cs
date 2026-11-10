using AutoMapper;
using Store.Core.DTO.Photo;
using Store.Core.DTO.Product;
using Store.Core.Entities;

namespace Store.API.Mapping;

public class ProductMapping : Profile
{
    public ProductMapping()
    {
        CreateMap<Product, ProductDto>()
            .ForCtorParam(nameof(ProductDto.CategoryName), options =>
                options.MapFrom(source => source.Category == null ? string.Empty : source.Category.Name))
            .ForCtorParam(nameof(ProductDto.PhotoUrls), options =>
                options.MapFrom(source => source.Photos));

        CreateMap<CreateProductRequest, Product>(MemberList.Source)
            .ForSourceMember(source => source.PhotoId, options => options.DoNotValidate());

        CreateMap<UpdateProductRequest, Product>(MemberList.Source)
            .ForSourceMember(source => source.PhotoId, options => options.DoNotValidate());


        CreateMap<Photo, PhotoDto>()
            .ForCtorParam(nameof(PhotoDto.ImageName), options => options.MapFrom(source => source.Name));
    }
}
