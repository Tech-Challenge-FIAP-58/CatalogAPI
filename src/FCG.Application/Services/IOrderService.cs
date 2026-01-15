using FCG.Core.Core.Inputs;
using FCG.Core.Core.Web;

namespace FCG.Application.Services
{
    public interface IOrderService
    {
        Task<int> Create(OrderRegisterDto orderRegisterDto);
        Task<OrderResponseDto?> GetById(int id);
        Task<IApiResponse<bool>> Update(int id, OrderUpdateDto update);


    }
}
