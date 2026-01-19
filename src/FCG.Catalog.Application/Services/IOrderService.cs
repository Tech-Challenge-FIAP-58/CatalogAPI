using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Web;

namespace FCG.Catalog.Application.Services
{
    public interface IOrderService
    {
        Task<int> Create(OrderRegisterDto orderRegisterDto);
        Task<OrderResponseDto?> GetById(int id);
        Task<IApiResponse<bool>> Update(int id, OrderUpdateDto update);


    }
}
