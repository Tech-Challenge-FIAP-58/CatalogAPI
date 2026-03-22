using Microsoft.EntityFrameworkCore;
using FCG.Catalog.Infra.Context;
using FCG.Catalog.Domain.Models.Catalog;

namespace FCG.Catalog.Infra.Repository
{
    public class GameRepository(ApplicationDbContext context)
        : Repository<Game>(context), IGameRepository
    {
        public Guid Create(Game game)
        {
            Add(game);
            return game.Id;
        }

        public Task<IEnumerable<Game>> GetAll() => base.GetAll();

        public Task<Game?> GetById(Guid id) => base.GetById(id);

        public async Task<Game?> GetByName(string name)
            => await _dbSet.AsNoTracking().Where(u => u.Name == name)
                .FirstOrDefaultAsync();

        public void Update(Game game)
        {
            base.Update(game);
        }

        public void Remove(Game game)
        {
            Delete(game);
        }
    }
}
