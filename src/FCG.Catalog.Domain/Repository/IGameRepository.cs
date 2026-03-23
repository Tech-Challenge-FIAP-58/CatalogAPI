using FCG.Catalog.Domain.Models.Catalog;

namespace FCG.Catalog.Domain.Repository
{
    public interface IGameRepository : IRepository<Game>
    {
        Guid Create(Game game);
        Task<IEnumerable<Game>> GetAll();
        Task<Game?> GetById(Guid id);
        Task<Game?> GetByName(string name);
        void Update(Game game);
        void Remove(Game game);
    }
}
