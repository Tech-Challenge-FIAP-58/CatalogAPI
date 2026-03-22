using FCG.Catalog.Domain.Events;

namespace FCG.Catalog.Domain.Models.Library
{
    public sealed class GameLibraryItem
    {
        public Guid GameId { get; private set; }
        public string Name { get; private set; }
        public string Platform { get; private set; }
        public string PublisherName { get; private set; }
        public string Description { get; private set; }
        public decimal UnitPrice { get; private set; }

        protected GameLibraryItem() { }

        public GameLibraryItem(Guid gameId, string name, string platform, string publisherName, string description, decimal unitPrice)
        {
            GameId = gameId;
            Name = name;
            Platform = platform;
            PublisherName = publisherName;
            Description = description;
            UnitPrice = unitPrice;
        }

        public GameLibraryGameSnapshot ToSnapshot() => new(GameId, Name, Platform, PublisherName, Description, UnitPrice);
    }
}
