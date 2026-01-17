using FCG.Core.Core.Inputs;
using FCG.Core.Core.Web;

namespace FCG.Application.Services
{
    public interface ICatalogService
    {
        Task<IApiResponse<IEnumerable<CatalogResponseDto>>> GetAll();
        Task<IApiResponse<CatalogResponseDto?>> GetByUserId(Guid id);
        Task<IApiResponse<CatalogRegisterDto>> Create(CatalogRegisterDto dto);
    }
}
