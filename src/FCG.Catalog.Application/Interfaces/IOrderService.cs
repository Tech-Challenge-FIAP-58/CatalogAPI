using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Web;
using FCG.Core.Integration;

namespace FCG.Catalog.Application.Interfaces
{
    public interface IOrderService
    {
        Task<IApiResponse<Guid?>> Create(OrderRegisterDto orderRegisterDto, CheckoutCartDto? checkoutDto = null);
        Task<IApiResponse<OrderResponseDto?>> GetById(Guid id);
        Task<IApiResponse<IReadOnlyCollection<OrderResponseDto>>> GetByUserId(int userId);
        Task<IApiResponse<bool>> UpdatePaymentStatus(Guid id, PaymentResultStatus paymentStatus);
        Task<IApiResponse<bool>> Update(Guid id, OrderUpdateDto update);
    }
}
