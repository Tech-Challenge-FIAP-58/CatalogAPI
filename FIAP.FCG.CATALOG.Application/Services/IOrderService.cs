using FIAP.FCG.CATALOG.Core.Inputs;
using FIAP.FCG.CATALOG.Core.Models;
using FIAP.FCG.CATALOG.Core.Web;

namespace FIAP.FCG.CATALOG.Application.Services
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
