using System.ComponentModel.DataAnnotations;
using FCG.Catalog.Application.Interfaces;
using FCG.Catalog.Domain.Models.Catalog;
using FCG.Catalog.Domain.Validation;
using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Repository;
using FCG.Catalog.Domain.Web;
using AutoMapper;

namespace FCG.Catalog.Application.Services
{
    public class GameService(IGameRepository _repository, IMapper _mapper) : BaseService, IGameCatalogLookupService, IGameReadService, IGameManagementService
    {
        public async Task<IApiResponse<Guid?>> Create(GameRegisterDto gameRegisterDto)
        {
            try
            {
                DtoValidator.ValidateObject(gameRegisterDto);
            }
            catch (ValidationException ex)
            {
                return BadRequest<Guid?>($"Invalid game data: {ex.Message}");
            }

            var gameExists = await _repository.GetByName(gameRegisterDto.Name);
            
            if (gameExists is not null)
                return BadRequest<Guid?>("Game already registered.");

            var game = _mapper.Map<Game>(gameRegisterDto);

            var id = _repository.Create(game);

            await _repository.SaveChangesAsync();

            return Created<Guid?>(id, "Game created successfully.");
        }

        public async Task<IApiResponse<bool>> Remove(Guid id)
        {
            var game = await _repository.GetById(id);

            if (game is null)
                return NotFound<bool>("Game not found for removal.");

            game.Delete();

            _repository.Remove(game); // No-op alignment

            await _repository.SaveChangesAsync();

            return NoContent();
        }

        public async Task<IApiResponse<IEnumerable<GameResponseDto>>> GetAll()
        {
            return Ok(_mapper.Map<IEnumerable<GameResponseDto>>(await _repository.GetAll()));
		}

        public async Task<IApiResponse<GameResponseDto?>> GetById(Guid id)
        {
            var game = await _repository.GetById(id);

            return game is null
                ? NotFound<GameResponseDto?>("Game not found.")
                : Ok<GameResponseDto?>(_mapper.Map<GameResponseDto>(game));
        }

        public async Task<GameLookupDto?> GetByIdForProcessing(Guid id)
        {
            var game = await _repository.GetById(id);

            return game is null
                ? null
                : new GameLookupDto(
                    game.Id,
                    game.Name,
                    game.Platform,
                    game.PublisherName,
                    game.Description,
                    game.Price,
                    game.IsAvailable);
        }

        public async Task<IApiResponse<bool>> Update(Guid id, GameUpdateDto updateDto)
        {
            var game = await _repository.GetById(id);

            if (game is null)
                return NotFound<bool>("Game not found for update.");

            game.Update(updateDto.Description, updateDto.Price, updateDto.IsAvailable);

            _repository.Update(game); // No-op alignment

            await _repository.SaveChangesAsync();
            
            return NoContent();
        }
    }
}
