using FCG.Core.Core.Inputs;
using FCG.Core.Core.Web;

namespace FCG.Application.Services
{
    public interface IOrderService
    {
        Task<Guid> Create(OrderRegisterDto orderRegisterDto);
        Task<OrderResponseDto?> GetById(Guid id);
        Task<IApiResponse<bool>> Update(Guid id, OrderUpdateDto update);


    }
}
