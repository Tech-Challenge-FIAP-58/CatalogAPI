using FCG.Catalog.Domain.Models.Library;
using FCG.Catalog.Domain.Repository;
using FCG.Catalog.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace FCG.Catalog.Infra.Repository
{
    public class GameLibraryRepository(ApplicationDbContext context)
        : Repository<GameLibrary>(context), IGameLibraryRepository
    {
        public Guid Create(GameLibrary library)
        {
            Add(library);
            return library.Id;
        }

        public async Task<GameLibrary?> GetByUserId(int userId)
            => await _dbSet.AsTracking()
                .Include(entity => entity.Games)
                .FirstOrDefaultAsync(entity => entity.UserId == userId);

        public void Update(GameLibrary library)
        {
            base.Update(library);
        }
    }
}
