using FCG.Catalog.Domain.Events;
using FCG.Catalog.Domain.Web;

namespace FCG.Catalog.Application.Interfaces
{
    public interface IGameLibraryOwnershipService
    {
        Task<IApiResponse<bool>> AddGames(int userId, IReadOnlyCollection<OrderItemSnapshot> games);
    }
}
