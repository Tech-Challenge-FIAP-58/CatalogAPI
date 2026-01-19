using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Web;

namespace FCG.Application.Services
{
    public interface ICatalogService
    {
        Task<IApiResponse<IEnumerable<CatalogResponseDto>>> GetAll();
        Task<IApiResponse<CatalogResponseDto?>> GetByUserId(int id);
        Task<IApiResponse<CatalogRegisterDto>> Create(CatalogRegisterDto dto);
    }
}
