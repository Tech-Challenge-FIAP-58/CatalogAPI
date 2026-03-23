using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Web;

namespace FCG.Catalog.Application.Interfaces
{
    public interface ICartReadService
    {
        Task<IApiResponse<CartResponseDto?>> GetByUserId(int userId);
    }
}
