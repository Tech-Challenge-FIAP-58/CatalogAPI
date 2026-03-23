using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Web;

namespace FCG.Catalog.Application.Interfaces
{
    public interface IOrderCheckoutService
    {
        Task<IApiResponse<Guid?>> Create(OrderRegisterDto orderRegisterDto, CheckoutCartDto? checkoutDto = null);
    }
}
