namespace SalesDb;

public record PersonDto(
    long Id,
    string FirstName,
    string LastName,
    string? Patronymic,
    string? PhoneNumber,
    string? Email,
    string? Address);