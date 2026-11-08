public record CreateProductRequest(string Name, string Description, decimal Price);
public record UpdateProductRequest(int Id, string Name, string Description, decimal Price);
public record ProductDto(int Id, string Name, string Description, decimal Price);