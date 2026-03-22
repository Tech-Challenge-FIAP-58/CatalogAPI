using FCG.Catalog.Domain.Mediatr;

namespace FCG.Catalog.Domain.Events;

public class GameCreatedDomainEvent : Event
{
    public GameSnapshot Game { get; }

    public GameCreatedDomainEvent(GameSnapshot game)
    {
        Game = game;
    }
}

public class GameUpdatedDomainEvent : Event
{
    public GameSnapshot Game { get; }

    public GameUpdatedDomainEvent(GameSnapshot game)
    {
        Game = game;
    }
}

public class GameDeletedDomainEvent : Event
{
    public GameSnapshot Game { get; }

    public GameDeletedDomainEvent(GameSnapshot game)
    {
        Game = game;
    }
}
