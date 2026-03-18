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

        private Order(DateTime orderDate, int userId, List<Game> orderGames)
        {   
            OrderDate = orderDate;
            UserId = userId;
            Status = OrderStatus.Authorized;
            _orderGames = orderGames ?? new List<Game>();
        }

        public static Order Create(DateTime orderDate, int userId, List<Game> orderGames)
        {
            var order = new Order(orderDate, userId, orderGames);
            order.AuthorizeOrder();

            order.CalculateOrderTotal();
            order.AddEvent(new OrderCreatedDomainEvent());

            return order;
        }

        public void Update(DateTime? orderDate, int? userId, decimal? total, List<Game>? orderGames)
        {
            if (orderDate.HasValue) OrderDate = orderDate.Value;
            if (userId.HasValue) UserId = userId.Value;
            if (total.HasValue) Total = total.Value;

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

        public void RejectOrder()
        {
            Status = OrderStatus.Rejected;
        }

        public void CalculateOrderTotal()
        {
            Total = OrderGames.Sum(p => p.Price);
        }

    }
}
