using FCG.Catalog.Domain.Enums;

namespace FCG.Catalog.Domain.Events;

public sealed record GameSnapshot(
    Guid Id,
    string Name,
    string Platform,
    string PublisherName,
    string Description,
    decimal Price,
    bool IsAvailable,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime? DeletedAt,
    bool IsDeleted);

public sealed record OrderItemSnapshot(
    Guid GameId,
    string Name,
    string Platform,
    string PublisherName,
    string Description,
    decimal Price);

public sealed record OrderSnapshot(
    Guid Id,
    DateTime OrderDate,
    int UserId,
    decimal Total,
    OrderStatus Status,
    IReadOnlyCollection<OrderItemSnapshot> Items,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime? DeletedAt,
    bool IsDeleted);

public sealed record CartItemSnapshot(
    Guid GameId,
    string Name,
    string Platform,
    string PublisherName,
    string Description,
    decimal UnitPrice,
    int Quantity,
    decimal Total);

public sealed record CartSnapshot(
    Guid Id,
    int UserId,
    IReadOnlyCollection<CartItemSnapshot> Items,
    decimal Total,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime? DeletedAt,
    bool IsDeleted,
    CartStatus Status);
