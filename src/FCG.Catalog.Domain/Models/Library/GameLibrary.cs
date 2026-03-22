using FCG.Catalog.Domain.Common;
using FCG.Catalog.Domain.Events;
using FCG.Catalog.Domain.Mediatr;

namespace FCG.Catalog.Domain.Models.Library
{
    public class GameLibrary : EntityBase, IAggregateRoot
    {
        public int UserId { get; private set; }

        private readonly List<GameLibraryItem> _games;

        public IReadOnlyCollection<GameLibraryItem> Games => _games;

        protected GameLibrary()
        {
            _games = new List<GameLibraryItem>();
        }

        private GameLibrary(int userId)
        {
            UserId = userId;
            _games = new List<GameLibraryItem>();
        }

        public static GameLibrary Create(int userId)
        {
            var library = new GameLibrary(userId);
            library.AddEvent(new GameLibraryCreatedDomainEvent(library.ToSnapshot()));
            return library;
        }

        public void AddGames(IReadOnlyCollection<OrderItemSnapshot> games)
        {
            var hasChanges = false;

            foreach (var game in games)
            {
                if (_games.Any(existing => existing.GameId == game.GameId))
                {
                    continue;
                }

                _games.Add(new GameLibraryItem(
                    game.GameId,
                    game.Name,
                    game.Platform,
                    game.PublisherName,
                    game.Description,
                    game.Price));

                hasChanges = true;
            }

            if (hasChanges)
            {
                AddEvent(new GameLibraryUpdatedDomainEvent(ToSnapshot()));
            }
        }

        public override Event CreateDomainEvent(DomainEventAction action) => action switch
        {
            DomainEventAction.Created => new GameLibraryCreatedDomainEvent(ToSnapshot()),
            DomainEventAction.Updated => new GameLibraryUpdatedDomainEvent(ToSnapshot()),
            DomainEventAction.Deleted => new GameLibraryDeletedDomainEvent(ToSnapshot()),
            _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
        };

        private GameLibrarySnapshot ToSnapshot() => new(
            Id,
            UserId,
            _games.Select(game => game.ToSnapshot()).ToList(),
            CreatedAt,
            UpdatedAt,
            DeletedAt,
            IsDeleted);
    }
}
