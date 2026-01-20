using FCG.Catalog.Application.Interfaces;
using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Validation;
using FCG.Catalog.Domain.Web;
using FCG.Catalog.Infra.Repository;
using System.ComponentModel.DataAnnotations;

namespace FCG.Catalog.Application.Services
{
    public class GameService(IGameRepository repository) : BaseService, IGameService
    {
        public async Task<IApiResponse<Guid>> Create(GameRegisterDto gameRegisterDto)
        {
            try
            {
                DtoValidator.ValidateObject(gameRegisterDto);
            }
            catch (ValidationException ex)
            {
                return BadRequest<Guid>($"Dados de jogo inválidos: {ex.Message}");
            }

            var id = await repository.Create(gameRegisterDto);
            return Created(id, "Jogo registrado com sucesso.");
        }

        public async Task<IApiResponse<bool>> Remove(Guid id)
        {
            var removed = await repository.Remove(id);
            return removed
                ? NoContent()
                : NotFound<bool>("Jogo não encontrado para remoção.");
        }

        public async Task<IApiResponse<IEnumerable<GameResponseDto>>> GetAll()
        {
            return Ok(await repository.GetAll());
		}

        public async Task<IApiResponse<GameResponseDto?>> GetById(Guid id)
        {
            var dto = await repository.GetById(id);
            return dto is null
                ? NotFound<GameResponseDto?>("Jogo não encontrado.")
                : Ok<GameResponseDto?>(dto);
        }

        public async Task<IApiResponse<bool>> Update(Guid id, GameUpdateDto updateDto)
        {
            var ok = await repository.Update(id, updateDto);
            return ok
                ? NoContent()
                : NotFound<bool>("Usuário não encontrado para atualização.");
        }
    }
}
