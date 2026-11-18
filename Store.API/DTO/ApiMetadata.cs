using System.Text.Json.Serialization;
using Store.API.Helper;

namespace Store.API.DTO;

public sealed record ApiMetadata(
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    PaginationMetadata? Pagination = null,

    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? TraceId = null
    );