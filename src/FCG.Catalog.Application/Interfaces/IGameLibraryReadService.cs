using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Web;

namespace FCG.Catalog.Application.Interfaces
{
    public interface IGameLibraryReadService
    {
        Task<IApiResponse<IReadOnlyCollection<GameLibraryGameResponseDto>>> GetGamesByUserId(int userId);
    }
}
