using FCG.Catalog.Domain.Events;

namespace FCG.Catalog.Domain.Models;

public class Game : EntityBase
{
    public string Name { get; private set; }
    public string Platform { get; private set; }
    public string PublisherName { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }

    protected Game() { }

    public Game(string name, string platform, string publisherName, string description, decimal price)
    {
        Name = name;
        Platform = platform;
        PublisherName = publisherName;
        Description = description;
        Price = price;
    }

    public static Game Create(string name, string platform, string publisherName, string description, decimal price)
    {
        var game = new Game
        {
            Name = name,
            Platform = platform,
            PublisherName = publisherName,
            Description = description,
            Price = price
        };

        game.AddEvent(new GameCreatedDomainEvent(name));

        return game;
    }

    public static Game Rehydrate(Guid id, string name, string platform, string publisherName, string description, decimal price, DateTime createdAtUtc)
    {
        var game = new Game(name, platform, publisherName, description, price)
        {
            Id = id,
            CreatedAt = createdAtUtc
        };

        return game;
    }

    public void Delete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        AddEvent(new GameDeletedDomainEvent(Name));
    }

    public void Update(string? name, string? platform, string? publisherName, string? description, decimal? price)
    {
        if (name is not null) Name = name;
        if (platform is not null) Platform = platform;
        if (publisherName is not null) PublisherName = publisherName;
        if (description is not null) Description = description;
        if (price.HasValue) Price = price.Value;
        UpdatedAt = DateTime.UtcNow;
        AddEvent(new GameUpdatedDomainEvent(Name));
    }
}
