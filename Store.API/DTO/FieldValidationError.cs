namespace Store.API.DTO;

public sealed record FieldValidationError(string? Field, string Message);