using AutoMapper;
using FCG.Catalog.Application.Interfaces;
using FCG.Catalog.Domain.Events;
using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Models.Library;
using FCG.Catalog.Domain.Web;
using FCG.Catalog.Infra.Caching;
using FCG.Catalog.Infra.Repository;

namespace FCG.Catalog.Application.Services
{
    public class GameLibraryService(IGameLibraryRepository repository, IMapper mapper, ICachingService cachingService) : BaseService, IGameLibraryService
    {
        public async Task<IApiResponse<bool>> AddGames(int userId, IReadOnlyCollection<OrderItemSnapshot> games)
        {
            if (games.Count == 0)
            {
                return NoContent();
            }

			var cacheKey = $"user:{userId}:library";

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
            await cachingService.SetAsync(cacheKey, library);

			return NoContent();
        }

        public async Task<IApiResponse<IReadOnlyCollection<GameLibraryGameResponseDto>>> GetGamesByUserId(int userId)
        {
            var cacheKey = $"user:{userId}:library";
            var cachedLibrary = await cachingService.GetAsync<GameLibrary>(cacheKey);
            if (cachedLibrary != null)
            {
				var cachedGames = mapper.Map<IReadOnlyCollection<GameLibraryGameResponseDto>>(cachedLibrary.Games);
				return Ok(cachedGames);
			}

			var library = await repository.GetByUserId(userId);

            if (library is null)
            {
                return Ok<IReadOnlyCollection<GameLibraryGameResponseDto>>([]);
            }

            var games = mapper.Map<IReadOnlyCollection<GameLibraryGameResponseDto>>(library.Games);

			await cachingService.SetAsync(cacheKey, library);
			return Ok(games);
        }
    }
}
