namespace SalesDb;

public record ProductDto(
    long Id,
    string Name,
    string Quantity,
    decimal Price);