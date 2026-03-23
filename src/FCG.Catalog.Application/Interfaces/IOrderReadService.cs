using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Web;

namespace FCG.Catalog.Application.Interfaces
{
    public interface IOrderReadService
    {
        Task<IApiResponse<OrderResponseDto?>> GetById(Guid id);
        Task<IApiResponse<IReadOnlyCollection<OrderResponseDto>>> GetByUserId(int userId);
    }
}
