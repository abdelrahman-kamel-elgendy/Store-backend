using Store.Core.DTO.Image;

namespace Store.Core.DTO.Product;

public record ProductResponseDto(
    int Id,
    string Name,
    string Description,
    decimal NewPrice,
    decimal OldPrice,
    string CategoryName,
    List<ImageResponsDto> ImageUrls,
    bool IsDeleted,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime? DeletedAt
    ) : IIdentifiableDto;