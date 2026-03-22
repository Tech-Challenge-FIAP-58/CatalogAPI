using FCG.Catalog.Domain.Common;
using FCG.Catalog.Domain.Enums;
using FCG.Catalog.Domain.Events;
using FCG.Catalog.Domain.Mediatr;
using System.Linq;

namespace FCG.Catalog.Domain.Models.Cart
{
    public class Cart : EntityBase, IAggregateRoot
    {
        public int UserId { get; private set; }
        public decimal Total { get; private set; }
        public CartStatus Status { get; private set; }

        private readonly List<CartItem> _items;

        public IReadOnlyCollection<CartItem> Items => _items;

        protected Cart()
        {
            _items = new List<CartItem>();
        }

        private Cart(int userId)
        {
            UserId = userId;
            Status = CartStatus.Active;
            _items = new List<CartItem>();
        }

        public static Cart Create(int userId)
        {
            var cart = new Cart(userId);
            cart.AddEvent(new CartCreatedDomainEvent(cart.ToSnapshot()));
            return cart;
        }

        public void AddItem(Guid gameId, string name, string platform, string publisherName, string description, decimal unitPrice)
        {
            if (Status == CartStatus.Completed)
            {
                return;
            }

            var existing = _items.FirstOrDefault(item => item.GameId == gameId);

            if (existing is not null)
            {
                throw new ArgumentException("Game is already added to cart.", nameof(gameId));
            }

            var item = new CartItem(gameId, name, platform, publisherName, description, unitPrice);
            _items.Add(item);

            RecalculateTotal();
            AddEvent(new CartUpdatedDomainEvent(ToSnapshot()));
            AddEvent(new CartItemAddedDomainEvent(ToSnapshot(), item.ToSnapshot()));
        }

        public void RemoveItem(Guid gameId)
        {
            if (Status == CartStatus.Completed)
            {
                return;
            }

            var existing = _items.FirstOrDefault(item => item.GameId == gameId);

            if (existing is null)
            {
                return;
            }

            var itemSnapshot = existing.ToSnapshot();
            _items.Remove(existing);
            RecalculateTotal();
            AddEvent(new CartUpdatedDomainEvent(ToSnapshot()));
            AddEvent(new CartItemRemovedDomainEvent(ToSnapshot(), itemSnapshot));
        }

        public void Clear()
        {
            if (Status == CartStatus.Completed)
            {
                return;
            }

            _items.Clear();
            RecalculateTotal();
            AddEvent(new CartUpdatedDomainEvent(ToSnapshot()));
        }

        public void Complete()
        {
            if (Status == CartStatus.Completed)
            {
                return;
            }

            Status = CartStatus.Completed;
            AddEvent(new CartUpdatedDomainEvent(ToSnapshot()));
        }

        public void Delete()
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            AddEvent(new CartDeletedDomainEvent(ToSnapshot()));
        }

        public override Event CreateDomainEvent(DomainEventAction action) => action switch
        {
            DomainEventAction.Created => new CartCreatedDomainEvent(ToSnapshot()),
            DomainEventAction.Updated => new CartUpdatedDomainEvent(ToSnapshot()),
            DomainEventAction.Deleted => new CartDeletedDomainEvent(ToSnapshot()),
            _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
        };

        private void RecalculateTotal()
        {
            Total = _items.Sum(item => item.Total);
        }

        private CartSnapshot ToSnapshot() => new(
            Id,
            UserId,
            _items.Select(item => item.ToSnapshot()).ToList(),
            Total,
            CreatedAt,
            UpdatedAt,
            DeletedAt,
            IsDeleted,
            Status);
    }
}
