using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Web;

namespace FCG.Catalog.Application.Interfaces
{
    public interface IOrderService
    {
        Task<IApiResponse<int>> Create(OrderRegisterDto orderRegisterDto);
        Task<OrderResponseDto?> GetById(int id);
        Task<IApiResponse<bool>> Update(int id, OrderUpdateDto update);
    }
}
