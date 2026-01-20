using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Web;

namespace FCG.Catalog.Application.Interfaces
{
    public interface IOrderService
    {
        Task<IApiResponse<Guid>> Create(OrderRegisterDto orderRegisterDto);
        Task<OrderResponseDto?> GetById(Guid id);
        Task<IApiResponse<bool>> Update(Guid id, OrderUpdateDto update);
    }
}
