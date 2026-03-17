using FCG.Catalog.Domain.Enums;
using FCG.Catalog.Domain.Events;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace FCG.Catalog.Domain.Models
{
    public class Order : EntityBase
    {
        public DateTime OrderDate { get; private set; }
        public int UserId { get; private set; }
        public decimal Total { get; private set; }
        public OrderStatus Status { get; private set; }

        private readonly List<Game> _orderGames;

        public IReadOnlyCollection<Game> OrderGames => _orderGames;

        protected Order() { }

        private Order(DateTime orderDate, int userId, OrderStatus status, List<Game> orderGames)
        {
            OrderDate = orderDate;
            UserId = userId;
            Status = status;
            _orderGames = orderGames ?? new List<Game>();
        }

        public static Order Create(DateTime orderDate, int userId, OrderStatus status, List<Game> orderGames)
        {
            var order = new Order(orderDate, userId, status, orderGames);

            order.CalculateOrderTotal();

            order.AddEvent(new OrderCreatedDomainEvent());
            return order;
        }

        public void Update(DateTime? orderDate, int? userId, decimal? total, OrderStatus? status, List<Game>? orderGames)
        {
            if (orderDate.HasValue) OrderDate = orderDate.Value;
            if (userId.HasValue) UserId = userId.Value;
            if (total.HasValue) Total = total.Value;
            if (status.HasValue) Status = status.Value;

            if (orderGames is not null)
            {
                _orderGames.Clear();
                _orderGames.AddRange(orderGames);
            }

            AddEvent(new OrderUpdatedDomainEvent());
        }

        public void AuthorizeOrder()
        {
            Status = OrderStatus.Authorized;
        }

        public void CancelOrder()
        {
            Status = OrderStatus.Cancelled;
        }

        public void PayOrder()
        {
            Status = OrderStatus.Paid;
        }

        public void CalculateOrderTotal()
        {
            Total = OrderGames.Sum(p => p.Price);
        }

    }
}
