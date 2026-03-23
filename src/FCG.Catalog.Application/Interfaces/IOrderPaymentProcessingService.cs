using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Web;
using FCG.Core.Integration;

namespace FCG.Catalog.Application.Interfaces
{
    public interface IOrderPaymentProcessingService
    {
        Task<IApiResponse<OrderResponseDto?>> GetById(Guid id);
        Task<IApiResponse<bool>> UpdatePaymentStatus(Guid id, PaymentResultStatus paymentStatus);
    }
}
