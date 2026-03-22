using FCG.Catalog.Domain.Events;
using FCG.Catalog.Domain.Models.Catalog;

namespace FCG.Catalog.Domain.Inputs
{
    public class GameResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Platform { get; set; }
        public string PublisherName { get;set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedAtUtc { get; set; }

        public static Game ToGame(GameResponseDto dto)
        {
            return Game.Rehydrate(
                dto.Id,
                dto.Name,
                dto.Platform,
                dto.PublisherName,
                dto.Description,
                dto.Price,
                dto.IsAvailable,
                dto.CreatedAtUtc
            );
        }

        public static OrderItemSnapshot ToOrderItemSnapshot(GameResponseDto dto)
        {
            return new OrderItemSnapshot(
                dto.Id,
                dto.Name,
                dto.Platform,
                dto.PublisherName,
                dto.Description,
                dto.Price
            );
        }
    }
}
