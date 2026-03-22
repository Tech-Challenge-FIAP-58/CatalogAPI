using FCG.Catalog.Domain.Events;
using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Web;

namespace FCG.Catalog.Application.Interfaces
{
    public interface IGameLibraryService
    {
        Task<IApiResponse<IReadOnlyCollection<GameLibraryGameResponseDto>>> GetGamesByUserId(int userId);
        Task<IApiResponse<bool>> AddGames(int userId, IReadOnlyCollection<OrderItemSnapshot> games);
    }
}
