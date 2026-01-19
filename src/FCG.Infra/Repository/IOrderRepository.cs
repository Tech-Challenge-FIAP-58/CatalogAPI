using FCG.Catalog.Domain.Inputs;

namespace FCG.Infra.Repository
{
    public interface IOrderRepository
    {
        Task<int> Create(OrderRegisterDto orderRegister);
        Task<OrderResponseDto?> GetById(int id);
        Task<bool> Update(int id, OrderUpdateDto orderUpdateDto);
    }
}
