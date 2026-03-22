using AutoMapper;
using FCG.Catalog.Application.Interfaces;
using FCG.Catalog.Domain.Events;
using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Models.Library;
using FCG.Catalog.Domain.Web;
using FCG.Catalog.Infra.Repository;

namespace FCG.Catalog.Application.Services
{
    public class GameLibraryService(IGameLibraryRepository repository, IMapper mapper) : BaseService, IGameLibraryService
    {
        public async Task<IApiResponse<bool>> AddGames(int userId, IReadOnlyCollection<OrderItemSnapshot> games)
        {
            if (games.Count == 0)
            {
                return NoContent();
            }

            var library = await repository.GetByUserId(userId);
            var isNewLibrary = library is null;

            if (isNewLibrary)
            {
                library = GameLibrary.Create(userId);
            }

            library.AddGames(games);

            if (isNewLibrary)
            {
                repository.Create(library);
            }
            else
            {
                repository.Update(library);
            }

            await repository.SaveChangesAsync();

            return NoContent();
        }

        public async Task<IApiResponse<IReadOnlyCollection<GameLibraryGameResponseDto>>> GetGamesByUserId(int userId)
        {
            var library = await repository.GetByUserId(userId);

            if (library is null)
            {
                return Ok<IReadOnlyCollection<GameLibraryGameResponseDto>>(Array.Empty<GameLibraryGameResponseDto>());
            }

            return Ok<IReadOnlyCollection<GameLibraryGameResponseDto>>(mapper.Map<IReadOnlyCollection<GameLibraryGameResponseDto>>(library.Games));
        }
    }
}
