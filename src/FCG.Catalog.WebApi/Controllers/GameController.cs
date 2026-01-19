using FCG.Application.Services;
using FCG.Catalog.Domain.Inputs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.WebApi.Controllers
{
    [Authorize]
    public class GameController(IGameService service, ILogger<GameController> logger) : StandardController
    {
        [Authorize(Roles = "Admin")]
        [HttpPost("RegisterGame")]
        [AllowAnonymous]
        public Task<IActionResult> Post([FromBody] GameRegisterDto register)
        {
            logger.LogInformation("POST - Criar jogo");
            return TryMethodAsync(() => service.Create(register), logger);
        }

        [HttpGet("GetAllGames")]
        public Task<IActionResult> Get()
        {
			logger.LogInformation("GET - Listar jogos");
			return TryMethodAsync(() => service.GetAll(), logger);
		}

        [HttpGet("GetGameById{id}")]
        public Task<IActionResult> GetById(int id)
        {
			logger.LogInformation("GET - Listar jogo por ID: {Id}", id);
			return TryMethodAsync(() => service.GetById(id), logger);
		}

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateGame/{id:int}")]
        public Task<IActionResult> Put(int id, [FromBody] GameUpdateDto update)
        {
            logger.LogInformation("PUT - Atualizar jogo com ID: {Id}", id);
            return TryMethodAsync(() => service.Update(id, update), logger);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteGame/{id:int}")]
        public Task<IActionResult> Delete(int id)
        {
            logger.LogInformation("DELETE - Excluir jogo com ID: {Id}", id);
            return TryMethodAsync(() => service.Remove(id), logger);
        }
    }
}
