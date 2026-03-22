using FCG.Catalog.Infra.Context;
using Microsoft.EntityFrameworkCore;
using OrderAggregate = FCG.Catalog.Domain.Models.Order.Order;

namespace FCG.Catalog.Infra.Repository
{
    public class OrderRepository(ApplicationDbContext context) : Repository<OrderAggregate>(context), IOrderRepository
    {
        public Guid Create(OrderAggregate order)
        {
            Add(order);
            return order.Id;
        }

        public async Task<OrderAggregate?> GetById(Guid id)
        {
            var order = await _dbSet.AsTracking()
                .Include(entity => entity.Items)
                .FirstOrDefaultAsync(entity => entity.Id == id);

            return order;
        }

        public async Task<IReadOnlyCollection<OrderAggregate>> GetByUserId(int userId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(entity => entity.Items)
                .Where(entity => entity.UserId == userId)
                .OrderByDescending(entity => entity.OrderDate)
                .ToListAsync();
        }

        public void Update(Guid id, OrderAggregate order)
        {
            base.Update(order);
        }
    }
}
