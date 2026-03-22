using FCG.Catalog.Domain.Enums;
using FCG.Catalog.Domain.Events;

namespace FCG.Catalog.Domain.Inputs
{
    public sealed record OrderResponseDto(
        Guid Id,
        DateTime OrderDate,
        int UserId,
        decimal Total,
        OrderStatus Status,
        IReadOnlyCollection<OrderItemSnapshot> Items
    );
}
