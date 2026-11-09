namespace Store.Core.DTO;

public record CreateCategoryRequest(string Name, string Description);
public record UpdateCategoryRequest(int Id, string Name, string Description);
public record CategoryDto(int Id, string Name, string Description);
