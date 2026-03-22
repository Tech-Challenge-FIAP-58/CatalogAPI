using FCG.Catalog.Domain.Models.Cart;

namespace FCG.Catalog.Infra.Repository
{
    public interface ICartRepository : IRepository<Cart>
    {
        Task<Cart?> GetByUserId(int userId);
        Guid Create(Cart cart);
        void Update(Cart cart);
    }
}
