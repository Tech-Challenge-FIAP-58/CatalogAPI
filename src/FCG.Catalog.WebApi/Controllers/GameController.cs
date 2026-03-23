using FCG.Catalog.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using FCG.Catalog.Domain.Inputs;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Catalog.WebApi.Controllers
{
    public class GameController(IGameReadService readService, IGameManagementService managementService, ILogger<GameController> logger) : StandardController
    {
        [Authorize(Roles = "Admin")]
        [HttpPost("RegisterGame")]
        public Task<IActionResult> Create([FromBody] GameRegisterDto register)
        {
            logger.LogInformation("POST - Create game");
            return TryMethodAsync(() => managementService.Create(register), logger);
        }

        [Authorize]
        [HttpGet("GetAllGames")]
        public Task<IActionResult> Get()
        {
            logger.LogInformation("GET - List games");
            return TryMethodAsync(() => readService.GetAll(), logger);
        }

        [Authorize]
        [HttpGet("GetGameById/{id:guid}")]
        public Task<IActionResult> GetById(Guid id)
        {
            logger.LogInformation("GET - Get game by ID: {Id}", id);
            return TryMethodAsync(() => readService.GetById(id), logger);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateGame/{id:guid}")]
        public Task<IActionResult> Update(Guid id, [FromBody] GameUpdateDto update)
        {
            logger.LogInformation("PUT - Update game with ID: {Id}", id);
            return TryMethodAsync(() => managementService.Update(id, update), logger);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteGame/{id:guid}")]
        public Task<IActionResult> Delete(Guid id)
        {
            logger.LogInformation("DELETE - Delete game with ID: {Id}", id);
            return TryMethodAsync(() => managementService.Remove(id), logger);
        }
    }
}
