namespace FCG.Catalog.Domain.Inputs;

public sealed record CartItemResponseDto(
    Guid GameId,
    string Name,
    string Platform,
    string PublisherName,
    string Description,
    decimal UnitPrice,
    int Quantity,
    decimal Total
);
