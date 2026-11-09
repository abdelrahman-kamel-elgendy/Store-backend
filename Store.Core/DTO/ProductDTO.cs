namespace Store.Core.DTO;

public record CreateProductRequest(string Name, string Description, decimal Price, int CategoryId);
public record UpdateProductRequest(int Id, string Name, string Description, decimal Price, int CategoryId);
public record ProductDto(int Id, string Name, string Description, decimal Price, int CategoryId);