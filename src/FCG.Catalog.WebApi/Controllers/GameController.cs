using FCG.Catalog.Application.Interfaces;
using FCG.Catalog.Domain.Inputs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Catalog.WebApi.Controllers
{
    public class GameController(IGameService service, ILogger<GameController> logger) : StandardController
    {
        [Authorize(Roles = "Admin")]
        [HttpPost("RegisterGame")]
        public Task<IActionResult> Create([FromBody] GameRegisterDto register)
        {
            logger.LogInformation("POST - Criar jogo");
            return TryMethodAsync(() => service.Create(register), logger);
        }

		[Authorize]
		[HttpGet("GetAllGames")]
        public Task<IActionResult> Get()
        {
			logger.LogInformation("GET - Listar jogos");
			return TryMethodAsync(() => service.GetAll(), logger);
		}

		[Authorize]
		[HttpGet("GetGameById{id}")]
        public Task<IActionResult> GetById(Guid id)
        {
			logger.LogInformation("GET - Listar jogo por ID: {Id}", id);
			return TryMethodAsync(() => service.GetById(id), logger);
		}

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateGame/{id:guid}")]
        public Task<IActionResult> Update(Guid id, [FromBody] GameUpdateDto update)
        {
            logger.LogInformation("PUT - Atualizar jogo com ID: {Id}", id);
            return TryMethodAsync(() => service.Update(id, update), logger);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteGame/{id:guid}")]
        public Task<IActionResult> Delete(Guid id)
        {
            logger.LogInformation("DELETE - Excluir jogo com ID: {Id}", id);
            return TryMethodAsync(() => service.Remove(id), logger);
        }
    }
}
