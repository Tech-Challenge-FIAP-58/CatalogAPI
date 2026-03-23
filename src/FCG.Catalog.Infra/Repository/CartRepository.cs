using FCG.Catalog.Domain.Models.Cart;
using FCG.Catalog.Domain.Enums;
using FCG.Catalog.Domain.Repository;
using FCG.Catalog.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace FCG.Catalog.Infra.Repository
{
    public class CartRepository(ApplicationDbContext context)
        : Repository<Cart>(context), ICartRepository
    {
        public Guid Create(Cart cart)
        {
            Add(cart);
            return cart.Id;
        }

        public async Task<Cart?> GetByUserId(int userId)
            => await _dbSet.AsTracking()
                .Include(entity => entity.Items)
                .FirstOrDefaultAsync(entity => entity.UserId == userId && entity.Status == CartStatus.Active);

        public void Update(Cart cart)
        {
            base.Update(cart);
        }
    }
}
