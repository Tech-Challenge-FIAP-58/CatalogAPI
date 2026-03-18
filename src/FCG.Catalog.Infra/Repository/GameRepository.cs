using Microsoft.EntityFrameworkCore;
using FCG.Catalog.Domain.Models;
using FCG.Catalog.Infra.Context;

namespace FCG.Catalog.Infra.Repository
{
    public class GameRepository(ApplicationDbContext context)
        : EFRepository<Game>(context), IGameRepository
    {
        public async Task<Guid> Create(Game game)
        {
            await Register(game);
            return game.Id;
        }

        public async Task<IEnumerable<Game>> GetAll() => await Get();

        public async Task<Game?> GetById(Guid id) => await _dbSet.Include(x => x.);

        public async Task<Game?> GetByName(string name)
            => await _dbSet.AsNoTracking().Where(u => u.Name == name)
                .FirstOrDefaultAsync();

        public async Task<bool> Update(Game game) => await Edit(game);

        public async Task<bool> Remove(Game game) => await Delete(game);
    }
}
