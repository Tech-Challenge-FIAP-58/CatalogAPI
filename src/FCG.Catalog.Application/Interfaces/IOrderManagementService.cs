using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Web;

namespace FCG.Catalog.Application.Interfaces
{
    public interface IOrderManagementService
    {
        Task<IApiResponse<Guid?>> Create(OrderRegisterDto orderRegisterDto, CheckoutCartDto? checkoutDto = null);
        Task<IApiResponse<bool>> Update(Guid id, OrderUpdateDto update);
    }
}
