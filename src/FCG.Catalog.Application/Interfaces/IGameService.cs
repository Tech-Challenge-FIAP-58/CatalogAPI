using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Web;

namespace FCG.Catalog.Application.Interfaces
{
    public interface IGameService
    {
        Task<IApiResponse<IEnumerable<GameResponseDto>>> GetAll();
        Task<IApiResponse<GameResponseDto?>> GetById(Guid id);
        Task<IApiResponse<Guid>> Create(GameRegisterDto register);
        Task<IApiResponse<bool>> Update(Guid id, GameUpdateDto update);
        Task<IApiResponse<bool>> Remove(Guid id);
    }
}
