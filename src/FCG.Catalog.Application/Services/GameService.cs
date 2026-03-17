using System.ComponentModel.DataAnnotations;
using FCG.Catalog.Application.Interfaces;
using FCG.Catalog.Domain.Validation;
using FCG.Catalog.Infra.Repository;
using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Models;
using FCG.Catalog.Domain.Web;
using AutoMapper;

namespace FCG.Catalog.Application.Services
{
    public class GameService(IGameRepository _repository, IMapper _mapper) : BaseService, IGameService
    {
        public async Task<IApiResponse<Guid?>> Create(GameRegisterDto gameRegisterDto)
        {
            try
            {
                DtoValidator.ValidateObject(gameRegisterDto);
            }
            catch (ValidationException ex)
            {
                return BadRequest<Guid?>($"Dados de jogo inválidos: {ex.Message}");
            }

            var gameExists = await _repository.GetByName(gameRegisterDto.Name);
            
            if (gameExists is not null)
                return BadRequest<Guid?>("Jogo já cadastrado.");

            var game = _mapper.Map<Game>(gameRegisterDto);

            var id = await _repository.Create(game);

            return Created<Guid?>(id, "Jogo registrado com sucesso.");
        }

        public async Task<IApiResponse<bool>> Remove(Guid id)
        {
            var game = await _repository.GetById(id);

            if (game is null)
                return NotFound<bool>("Jogo não encontrado para remoção.");

            game.Delete();

            var removed = await _repository.Remove(game);

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
                ? NotFound<GameResponseDto?>("Jogo não encontrado.")
                : Ok<GameResponseDto?>(_mapper.Map<GameResponseDto>(game));
        }

        public async Task<IApiResponse<bool>> Update(Guid id, GameUpdateDto updateDto)
        {
            var game = await _repository.GetById(id);

            if (game is null)
                return NotFound<bool>("Jogo não encontrado para atualização.");

            game.Update(updateDto.Name, updateDto.Platform, updateDto.PublisherName, updateDto.Description, updateDto.Price);

            await _repository.Update(game);
            
            return NoContent();
        }
    }
}
