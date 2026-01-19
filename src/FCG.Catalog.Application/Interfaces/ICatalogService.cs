using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Web;

namespace FCG.Catalog.Application.Interfaces
{
    public interface ICatalogService
    {
        Task<IApiResponse<IEnumerable<CatalogResponseDto>>> GetAll();
        Task<IApiResponse<CatalogResponseDto?>> GetByUserId(int id);
        Task<IApiResponse<CatalogRegisterDto>> Create(CatalogRegisterDto dto);
    }
}
