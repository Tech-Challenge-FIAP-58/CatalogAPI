using FCG.Catalog.Domain.Common;
using FCG.Catalog.Domain.Events;
using FCG.Catalog.Domain.Mediatr;

namespace FCG.Catalog.Domain.Models.Catalog;

public class Game : EntityBase, IAggregateRoot
{
    public string Name { get; private set; }
    public string Platform { get; private set; }
    public string PublisherName { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public bool IsAvailable { get; private set; }

    protected Game() { }

    public Game(string name, string platform, string publisherName, string description, decimal price)
    {
        Id = Guid.NewGuid();
        Name = name;
        Platform = platform;
        PublisherName = publisherName;
        Description = description;
        Price = price;
        IsAvailable = true;
    }

    public static Game Create(string name, string platform, string publisherName, string description, decimal price)
    {
        var game = new Game
        {
            Name = name,
            Platform = platform,
            PublisherName = publisherName,
            Description = description,
            Price = price,
            IsAvailable = true
        };

        game.AddEvent(new GameCreatedDomainEvent(game.ToSnapshot()));

        return game;
    }

    public static Game Create(string name, string description, decimal price)
    {
        return Create(name, "N/A", "N/A", description, price);
    }

    public static Game Rehydrate(Guid id, string name, string platform, string publisherName, string description, decimal price, bool isAvailable, DateTime createdAtUtc)
    {
        var game = new Game(name, platform, publisherName, description, price)
        {
            Id = id,
            CreatedAt = createdAtUtc,
            IsAvailable = isAvailable
        };

        return game;
    }

    public void Delete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        AddEvent(new GameDeletedDomainEvent(ToSnapshot()));
    }

    public void Update(string? description, decimal? price, bool? isAvailable)
    {
        if (description is not null) Description = description;
        if (price.HasValue) Price = price.Value;
        if (isAvailable.HasValue) IsAvailable = isAvailable.Value;
        AddEvent(new GameUpdatedDomainEvent(ToSnapshot()));
    }

    public void ChangePrice(decimal price) => Update(null, price, null);

    public void ChangeDescription(string description) => Update(description, null, null);

    public void SetAvailability(bool isAvailable) => Update(null, null, isAvailable);

    public override Event CreateDomainEvent(DomainEventAction action) => action switch
    {
        DomainEventAction.Created => new GameCreatedDomainEvent(ToSnapshot()),
        DomainEventAction.Updated => new GameUpdatedDomainEvent(ToSnapshot()),
        DomainEventAction.Deleted => new GameDeletedDomainEvent(ToSnapshot()),
        _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
    };

    private GameSnapshot ToSnapshot() => new(
        Id,
        Name,
        Platform,
        PublisherName,
        Description,
        Price,
        IsAvailable,
        CreatedAt,
        UpdatedAt,
        DeletedAt,
        IsDeleted);
}
