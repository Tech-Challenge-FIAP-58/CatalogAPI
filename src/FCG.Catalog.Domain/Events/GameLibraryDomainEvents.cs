using FCG.Catalog.Domain.Mediatr;

namespace FCG.Catalog.Domain.Events;

public class GameLibraryCreatedDomainEvent : Event
{
    public GameLibrarySnapshot Library { get; }

    public GameLibraryCreatedDomainEvent(GameLibrarySnapshot library)
    {
        Library = library;
    }
}

public class GameLibraryUpdatedDomainEvent : Event
{
    public GameLibrarySnapshot Library { get; }

    public GameLibraryUpdatedDomainEvent(GameLibrarySnapshot library)
    {
        Library = library;
    }
}

public class GameLibraryDeletedDomainEvent : Event
{
    public GameLibrarySnapshot Library { get; }

    public GameLibraryDeletedDomainEvent(GameLibrarySnapshot library)
    {
        Library = library;
    }
}
