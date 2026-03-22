using OrderAggregate = FCG.Catalog.Domain.Models.Order.Order;

namespace FCG.Catalog.Infra.Repository
{
    public interface IOrderRepository : IRepository<OrderAggregate>
    {
        Guid Create(OrderAggregate orderRegister);
        Task<OrderAggregate?> GetById(Guid id);
        Task<IReadOnlyCollection<OrderAggregate>> GetByUserId(int userId);
        void Update(Guid id, OrderAggregate orderUpdateDto);
    }
}
