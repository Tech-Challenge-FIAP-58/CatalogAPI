using FCG.Catalog.Domain.Models.Library;

namespace FCG.Catalog.Infra.Repository
{
    public interface IGameLibraryRepository : IRepository<GameLibrary>
    {
        Guid Create(GameLibrary library);
        Task<GameLibrary?> GetByUserId(int userId);
        void Update(GameLibrary library);
    }
}
