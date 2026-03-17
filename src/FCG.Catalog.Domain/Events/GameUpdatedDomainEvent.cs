using FCG.Catalog.Domain.Mediatr;

namespace FCG.Catalog.Domain.Events;

public class GameUpdatedDomainEvent : Event
{
    public string GameName { get; private set; }

    public GameUpdatedDomainEvent(string gameName)
    {
        GameName = gameName;
    }
}

