using FCG.Catalog.Domain.Models;

namespace FCG.Catalog.Domain.Inputs;

public sealed record class OrderRegisterDto
{
    public required DateTime OrderDate { get; set; }
    public required int UserId { get; set; }
    public required List<GameResponseDto> OrderGames { get; set; }

    public static Order ToOrder(OrderRegisterDto dto)
    {
        return Order.Create(
            dto.OrderDate,
            dto.UserId,
            dto.OrderGames.Select(GameResponseDto.ToGame).ToList()
        );
    }
}
