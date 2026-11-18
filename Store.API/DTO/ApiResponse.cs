using System.Text.Json.Serialization;

namespace Store.API.DTO;

public sealed record ApiResponse<T>(
    bool Success,

    int StatusCode,

    string Message,

    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    T? Data,

    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    IReadOnlyList<FieldValidationError>? Errors = null,

    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    ApiMetadata? Meta = null
    );