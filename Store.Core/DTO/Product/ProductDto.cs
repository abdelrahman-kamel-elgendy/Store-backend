using Store.Core.DTO.Photo;

namespace Store.Core.DTO.Product;

public record ProductDto(int Id, string Name, decimal Price, string CategoryName, List<PhotoDto> PhotoUrls) : IIdentifiableDto;