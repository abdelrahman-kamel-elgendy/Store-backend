namespace Store.Core.DTO.Image;

public record ImageResponsDto(
    int Id,
    string Url,
    int ProductId,
    bool IsMain
    ) : IIdentifiableDto;