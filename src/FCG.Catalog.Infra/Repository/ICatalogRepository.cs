using FCG.Catalog.Domain.Inputs;

namespace FCG.Catalog.Infra.Repository
{
    public interface ICatalogRepository
    {
        Task<IEnumerable<CatalogResponseDto>> GetAll();
        Task<CatalogResponseDto?> GetByUserId(int id);
        Task<bool> Create(CatalogRegisterDto dto);
    }
}
