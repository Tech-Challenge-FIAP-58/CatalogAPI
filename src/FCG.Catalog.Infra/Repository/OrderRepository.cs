using FCG.Catalog.Domain.Models;
using FCG.Catalog.Infra.Context;

namespace FCG.Catalog.Infra.Repository
{
    public class OrderRepository(ApplicationDbContext context) : EFRepository<Order>(context), IOrderRepository
    {
        public async Task<Guid> Create(Order order)
        {
            await Register(order);
            return order.Id;
        }

        public async Task<Order?> GetById(Guid id)
        {
            var order = await Get(id);

            return order;
        }

        public async Task<bool> Update(Guid id, Order order)
        {
            return await Edit(order);
        }
    }
}
