using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Models;

namespace FCG.Catalog.Infra.Repository
{
    public interface IGameRepository
    {
        Task<Guid> Create(Game game);
        Task<IEnumerable<Game>> GetAll();
        Task<Game?> GetById(Guid id);
        Task<Game?> GetByName(string name);
        Task<bool> Update(Game game);
        Task<bool> Remove(Game game);

    }
}
