using FCG.Core.Core.Inputs;

namespace FCG.Infra.Repository
{
    public interface ICatalogRepository
    {
        Task<IEnumerable<CatalogResponseDto>> GetAll();
        Task<CatalogResponseDto?> GetByUserId(int id);
        Task<bool> Create(CatalogRegisterDto dto);
    }
}
