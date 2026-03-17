using FCG.Catalog.Domain.Mediatr;

namespace FCG.Catalog.Domain.Events;

public class GameDeletedDomainEvent : Event
{
    public string GameName { get; private set; }

    public GameDeletedDomainEvent(string gameName)
    {
        GameName = gameName;
    }
}
