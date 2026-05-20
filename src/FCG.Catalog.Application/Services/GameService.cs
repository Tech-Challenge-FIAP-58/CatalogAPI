using AutoMapper;
using FCG.Catalog.Application.Interfaces;
using FCG.Catalog.Application.Services;
using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Models.Catalog;
using FCG.Catalog.Domain.Validation;
using FCG.Catalog.Domain.Web;
using FCG.Catalog.Infra.Mongo;
using FCG.Catalog.Infra.Repository;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

public class GameService(
    IGameRepository _repository,
    IMapper _mapper,
    IGameSearchService _search,          
    ILogger<GameService> _logger)        
    : BaseService, IGameService
{
    public async Task<IApiResponse<Guid?>> Create(GameRegisterDto gameRegisterDto)
using AutoMapper;
using FCG.Catalog.Domain.Common;

namespace FCG.Catalog.Application.Services
{
    public class GameService(IGameRepository _repository, IMapper _mapper, IEventLogRepository eventLogRepository) : BaseService, IGameService
    {
        try { DtoValidator.ValidateObject(gameRegisterDto); }
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
            await _repository.SaveChangesAsync();
            await eventLogRepository.InsertGameEventLog(new GameEventLog
            {
                GameId = id.ToString(),
                Name = gameRegisterDto.Name,
                Message = "Novo jogo criado"
            });

        var doc = new GameDocument
        {
            Id = game.Id.ToString(),
            Name = game.Name ?? string.Empty,
            PublisherName = game.PublisherName ?? string.Empty,
            Description = game.Description ?? string.Empty,
            Platform = game.Platform ?? string.Empty,
            Price = (double)game.Price,
            IsAvailable = game.IsAvailable
        };

        try
        {
            await _search.IndexGameAsync(doc);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao indexar game {Id} no Elasticsearch.", game.Id);
        }

        return Created<Guid?>(id, "Game created successfully.");
    }

    public async Task<IApiResponse<bool>> Update(Guid id, GameUpdateDto updateDto)
    {
        var game = await _repository.GetById(id);
        if (game is null)
            return NotFound<bool>("Game not found for update.");

        game.Update(updateDto.Description, updateDto.Price, updateDto.IsAvailable);
        _repository.Update(game);
        await _repository.SaveChangesAsync();
            await _repository.SaveChangesAsync();
            await eventLogRepository.InsertGameEventLog(new GameEventLog
            {
                GameId = id.ToString(),
                Name = game.Name,
                Message = "Jogo removido"
            });

        try
        {
            var doc = new GameDocument
            {
                Id = game.Id.ToString(),
                Name = game.Name ?? string.Empty,
                PublisherName = game.PublisherName ?? string.Empty,
                Description = game.Description ?? string.Empty,
                Platform = game.Platform ?? string.Empty,
                Price = (double)game.Price,
                IsAvailable = game.IsAvailable
            };
            await _search.IndexGameAsync(doc);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao re-indexar game {Id}.", id);
        }

        return NoContent();
    }

    public async Task<IApiResponse<bool>> Remove(Guid id)
    {
        var game = await _repository.GetById(id);
        if (game is null)
            return NotFound<bool>("Game not found for removal.");

        game.Delete();
        _repository.Remove(game);
        await _repository.SaveChangesAsync();

        try
        {
            await _search.RemoveGameAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao remover game {Id} do Elasticsearch.", id);
        }

        return NoContent();
    }

    public async Task<IApiResponse<IEnumerable<GameResponseDto>>> GetAll() =>
        Ok(_mapper.Map<IEnumerable<GameResponseDto>>(await _repository.GetAll()));

    public async Task<IApiResponse<GameResponseDto?>> GetById(Guid id)
    {
        var game = await _repository.GetById(id);
        return game is null
            ? NotFound<GameResponseDto?>("Game not found.")
            : Ok<GameResponseDto?>(_mapper.Map<GameResponseDto>(game));
            await _repository.SaveChangesAsync();
            await eventLogRepository.InsertGameEventLog(new GameEventLog
            {
                GameId = game.Id.ToString(),
                Name = game.Name,
                Message = "Jogo atualizado"
            });
            
            return NoContent();
        }
    }
}