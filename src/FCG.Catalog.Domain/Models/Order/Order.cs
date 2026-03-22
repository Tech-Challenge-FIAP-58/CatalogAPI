using FCG.Catalog.Domain.Common;
using FCG.Catalog.Domain.Enums;
using FCG.Catalog.Domain.Events;
using FCG.Catalog.Domain.Mediatr;

namespace FCG.Catalog.Domain.Models.Order
{
    public class Order : EntityBase, IAggregateRoot
    {
        public DateTime OrderDate { get; private set; }
        public int UserId { get; private set; }
        public decimal Total { get; private set; }
        public OrderStatus Status { get; private set; }

        private readonly List<OrderItem> _items;

        public IReadOnlyCollection<OrderItem> Items => _items;

        protected Order()
        {
            _items = new List<OrderItem>();
        }

        private Order(DateTime orderDate, int userId, List<OrderItemSnapshot> orderItems)
        {   
            OrderDate = orderDate;
            UserId = userId;
            Status = OrderStatus.Authorized;

            if (orderItems is not null && orderItems.GroupBy(item => item.GameId).Any(group => group.Count() > 1))
            {
                throw new ArgumentException("Order already contains duplicated games.", nameof(orderItems));
            }

            _items = orderItems?.Select(item => new OrderItem(item.GameId, item.Name, item.Platform, item.PublisherName, item.Description, item.Price)).ToList() ?? new List<OrderItem>();
        }

        public static Order Create(DateTime orderDate, int userId, List<OrderItemSnapshot> orderItems)
        {
            var order = new Order(orderDate, userId, orderItems);
            order.AuthorizeOrder();

            order.CalculateOrderTotal();
            order.AddEvent(new OrderCreatedDomainEvent(order.ToSnapshot()));

            return order;
        }

        public void Update(DateTime? orderDate, int? userId, decimal? total, List<OrderItemSnapshot>? orderItems = null)
        {
            if (orderDate.HasValue) OrderDate = orderDate.Value;
            if (userId.HasValue) UserId = userId.Value;
            if (total.HasValue) Total = total.Value;

            if (orderItems is not null)
            {
                if (orderItems.GroupBy(item => item.GameId).Any(group => group.Count() > 1))
                {
                    throw new ArgumentException("Order already contains duplicated games.", nameof(orderItems));
                }

                _items.Clear();
                _items.AddRange(orderItems.Select(item => new OrderItem(item.GameId, item.Name, item.Platform, item.PublisherName, item.Description, item.Price)));
            }

            AddEvent(new OrderUpdatedDomainEvent(ToSnapshot()));
        }

        public void AddItem(OrderItemSnapshot item)
        {
            if (_items.Any(existing => existing.GameId == item.GameId))
            {
                throw new ArgumentException("Game is already added to order.", nameof(item));
            }

            _items.Add(new OrderItem(item.GameId, item.Name, item.Platform, item.PublisherName, item.Description, item.Price));
            CalculateOrderTotal();
            AddEvent(new OrderItemAddedDomainEvent(ToSnapshot(), item));
        }

        public void RemoveItem(Guid gameId)
        {
            var existing = _items.FirstOrDefault(item => item.GameId == gameId);

            if (existing is null)
            {
                return;
            }

            var itemSnapshot = existing.ToSnapshot();
            _items.Remove(existing);
            CalculateOrderTotal();
            AddEvent(new OrderItemRemovedDomainEvent(ToSnapshot(), itemSnapshot));
        }
        
        public void AuthorizeOrder()
        {
            if (Status == OrderStatus.Authorized)
            {
                return;
            }

            Status = OrderStatus.Authorized;
            AddEvent(new OrderUpdatedDomainEvent(ToSnapshot()));
        }

        public void CancelOrder()
        {
            if (Status == OrderStatus.Cancelled)
            {
                return;
            }

            Status = OrderStatus.Cancelled;
            AddEvent(new OrderUpdatedDomainEvent(ToSnapshot()));
        }

        public void PayOrder()
        {
            if (Status == OrderStatus.Paid)
            {
                return;
            }

            Status = OrderStatus.Paid;
            AddEvent(new OrderUpdatedDomainEvent(ToSnapshot()));
        }

        public void RejectOrder()
        {
            if (Status == OrderStatus.Rejected)
            {
                return;
            }

            Status = OrderStatus.Rejected;
            AddEvent(new OrderUpdatedDomainEvent(ToSnapshot()));
        }

        public void Delete()
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            AddEvent(new OrderDeletedDomainEvent(ToSnapshot()));
        }

        public void CalculateOrderTotal()
        {
            Total = _items.Sum(p => p.Total);
        }

        public override Event CreateDomainEvent(DomainEventAction action) => action switch
        {
            DomainEventAction.Created => new OrderCreatedDomainEvent(ToSnapshot()),
            DomainEventAction.Updated => new OrderUpdatedDomainEvent(ToSnapshot()),
            DomainEventAction.Deleted => new OrderDeletedDomainEvent(ToSnapshot()),
            _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
        };

        private OrderSnapshot ToSnapshot() => new(
            Id,
            OrderDate,
            UserId,
            Total,
            Status,
            _items.Select(item => item.ToSnapshot()).ToList(),
            CreatedAt,
            UpdatedAt,
            DeletedAt,
            IsDeleted);

    }
}
