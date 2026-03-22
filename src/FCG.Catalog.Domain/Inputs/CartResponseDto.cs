using FCG.Catalog.Domain.Enums;

namespace FCG.Catalog.Domain.Inputs;

public sealed record CartResponseDto(
    Guid Id,
    int UserId,
    decimal Total,
    IReadOnlyCollection<CartItemResponseDto> Items,
    CartStatus Status
);
