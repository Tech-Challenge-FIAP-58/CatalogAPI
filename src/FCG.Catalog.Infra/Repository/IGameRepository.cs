using FCG.Catalog.Domain.Inputs;

namespace FCG.Catalog.Infra.Repository
{
    public interface IGameRepository
    {
        Task<Guid> Create(GameRegisterDto gameRegister);
        Task<IEnumerable<GameResponseDto>> GetAll();
        Task<GameResponseDto?> GetById(Guid id);
        Task<bool> Update(Guid id, GameUpdateDto gameUpdateDto);
        Task<bool> Remove(Guid id);

    }
}
