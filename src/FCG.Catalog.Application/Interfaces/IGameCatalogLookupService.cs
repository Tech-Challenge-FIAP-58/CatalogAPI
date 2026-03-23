using FCG.Catalog.Domain.Inputs;

namespace FCG.Catalog.Application.Interfaces
{
    public interface IGameCatalogLookupService
    {
        Task<GameLookupDto?> GetByIdForProcessing(Guid id);
    }
}
