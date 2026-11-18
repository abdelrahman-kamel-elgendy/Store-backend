using AutoMapper;
using Store.Core.DTOs.Product;
using Store.Core.DTOs.Image; // Make sure this is included
using Store.Core.Entities;

namespace Store.Core.Mapping;

public class ProductMapping : Profile
{
    public ProductMapping()
    {
        CreateMap<Image, ImageResponseDto>();

        CreateMap<Product, ProductResponseDto>()
            .ForCtorParam(nameof(ProductResponseDto.CategoryName),
                opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
            .ForCtorParam(nameof(ProductResponseDto.ImageUrls),
                opt => opt.MapFrom(src => src.Images));

        CreateMap<ProductRequestDto, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.Images, opt => opt.Ignore());
    }
}