using FCG.Core.Core.Inputs;
using FCG.Core.Core.Web;

namespace FCG.Application.Services
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
