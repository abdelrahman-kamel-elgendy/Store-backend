using System;

namespace Store.Core.DTO.Photo;

public record PhotoDto(string ImageName, string Url, int ProductId, bool IsMain);
