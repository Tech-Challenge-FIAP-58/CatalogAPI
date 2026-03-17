using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Models;

namespace FCG.Catalog.Infra.Repository
{
    public interface IOrderRepository
    {
        Task<Guid> Create(Order orderRegister);
        Task<Order?> GetById(Guid id);
        Task<bool> Update(Guid id, Order orderUpdateDto);
    }
}
