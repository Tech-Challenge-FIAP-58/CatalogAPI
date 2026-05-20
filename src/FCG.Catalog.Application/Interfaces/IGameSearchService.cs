using System.Threading;
using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Models.Catalog;
using FCG.Catalog.Domain.Web;
using FCG.Catalog.Infra.Mongo;

namespace FCG.Catalog.Application.Interfaces
{
    public interface IGameSearchService
    {
        Task<IApiResponse<IEnumerable<GameResponseDto>>> Search(string query, int page = 1, int pageSize = 20, CancellationToken ct = default);
        Task IndexGameAsync(GameDocument doc, CancellationToken ct = default);
        Task RemoveGameAsync(Guid id, CancellationToken ct = default);
    }
}
