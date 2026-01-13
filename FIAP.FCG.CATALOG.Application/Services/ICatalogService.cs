using FIAP.FCG.CATALOG.Core.Inputs;
using FIAP.FCG.CATALOG.Core.Web;

namespace FIAP.FCG.CATALOG.Application.Services
{
	public interface ICatalogService
	{
		Task<IApiResponse<IEnumerable<CatalogResponseDto>>> GetAll();
        Task<IApiResponse<CatalogResponseDto?>> GetByUserId(int id);
        Task<IApiResponse<CatalogRegisterDto>> Create(CatalogRegisterDto dto);
    }
}
