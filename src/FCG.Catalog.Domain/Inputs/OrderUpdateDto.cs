using FCG.Catalog.Domain.Models;

namespace FCG.Catalog.Domain.Inputs
{
    public sealed record OrderUpdateDto
    {
        public DateTime OrderDate { get; set; }
        public int UserId { get; set; }
        public decimal Total { get; set; }
        public required List<GameResponseDto> OrderGames { get; set; }

        public static Order ToOrder(OrderUpdateDto dto)
        {
            var order = Order.Create(
                dto.OrderDate,
                dto.UserId,
                dto.OrderGames.Select(GameResponseDto.ToGame).ToList()
            );

            return order;
        }
    }
}
