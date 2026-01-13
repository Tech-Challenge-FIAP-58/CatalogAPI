using FIAP.FCG.CATALOG.Core.Inputs;
using FIAP.FCG.CATALOG.Core.Models;

namespace FIAP.FCG.CATALOG.Infra.Repository
{
	public interface IGameRepository
	{
		Task<int> Create(GameRegisterDto gameRegister);
        Task<IEnumerable<GameResponseDto>> GetAll();
		Task<GameResponseDto?> GetById(int id);
		Task<bool> Update(int id, GameUpdateDto gameUpdateDto);
		Task<bool> Remove(int id);

    }
}
