namespace SalesDb;

public record ProductDto(
    long Id,
    string Name,
    long Quantity,
    double Price);