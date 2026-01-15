using FCG.Core.Core.Inputs;
using FCG.Core.Core.Web;

namespace FCG.Application.Services
{
    public interface IGameService
    {
        Task<IApiResponse<IEnumerable<GameResponseDto>>> GetAll();
        Task<IApiResponse<GameResponseDto?>> GetById(int id);
        Task<IApiResponse<int>> Create(GameRegisterDto register);
        Task<IApiResponse<bool>> Update(int id, GameUpdateDto update);
        Task<IApiResponse<bool>> Remove(int id);
    }
}
