using FCG.Catalog.Domain.Inputs;

namespace FCG.Catalog.Infra.Repository
{
    public interface IOrderRepository
    {
        Task<Guid> Create(OrderRegisterDto orderRegister);
        Task<OrderResponseDto?> GetById(Guid id);
        Task<bool> Update(Guid id, OrderUpdateDto orderUpdateDto);
    }
}
