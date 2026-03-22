using FCG.Catalog.Domain.Events;

namespace FCG.Catalog.Domain.Models.Order
{
    public sealed class OrderItem
    {
        public Guid GameId { get; private set; }
        public string Name { get; private set; }
        public string Platform { get; private set; }
        public string PublisherName { get; private set; }
        public string Description { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal Total => UnitPrice;

        protected OrderItem() { }

        public OrderItem(Guid gameId, string name, string platform, string publisherName, string description, decimal unitPrice)
        {
            GameId = gameId;
            Name = name;
            Platform = platform;
            PublisherName = publisherName;
            Description = description;
            UnitPrice = unitPrice;
        }

        public OrderItemSnapshot ToSnapshot() => new(GameId, Name, Platform, PublisherName, Description, UnitPrice);
    }
}
