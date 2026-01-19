using FCG.Catalog.Domain.Inputs;

namespace FCG.Catalog.Infra.Repository
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
