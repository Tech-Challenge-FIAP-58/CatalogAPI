using FIAP.FCG.CATALOG.Core.Inputs;

namespace FIAP.FCG.CATALOG.Infra.Repository
{
	public interface ICatalogRepository
	{
		Task<IEnumerable<CatalogResponseDto>> GetAll();
        Task<CatalogResponseDto?> GetByUserId(int id);
        Task<bool> Create(CatalogRegisterDto dto);
    }
}
