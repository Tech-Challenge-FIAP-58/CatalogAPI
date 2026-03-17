using FCG.Catalog.Domain.Mediatr;

namespace FCG.Catalog.Domain.Events;

public class GameCreatedDomainEvent : Event
{
    public string GameName { get; private set; }

    public GameCreatedDomainEvent(string gameName)
    {
        GameName = gameName;
    }
}
