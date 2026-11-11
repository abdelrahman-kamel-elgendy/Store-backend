using AutoMapper;
using Store.Core.DTO.Image;
using Store.Core.DTO.Product;
using Store.Core.Entities;

namespace Store.API.Mapping;

public class ProductMapping : Profile
{
    public ProductMapping()
    {
        // Entity -> Response
        CreateMap<Product, ProductResponseDto>()
            .ForCtorParam(nameof(ProductResponseDto.CategoryName), opt => opt.MapFrom(s => s.Category != null ? s.Category.Name : string.Empty))
            .ForCtorParam(nameof(ProductResponseDto.ImageUrls), opt => opt.MapFrom(s => s.Images));

        // Request -> Entity (create and update via Map(dto, entity))
        CreateMap<ProductRequestDto, Product>(MemberList.Source)
            // Files are handled by your upload service, not mapped to the entity
            .ForSourceMember(src => src.Images, opt => opt.DoNotValidate())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.Images, opt => opt.Ignore());

        // Image -> ImageResponsDto (Id, Url, ProductId, IsMain match by name)
        CreateMap<Image, ImageResponsDto>();
    }
}