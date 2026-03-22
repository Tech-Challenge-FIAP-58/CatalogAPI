using FCG.Catalog.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Catalog.WebApi.Controllers
{
    public class LibraryController(IGameLibraryService service, ILogger<LibraryController> logger) : StandardController
    {
        //[Authorize]
        [HttpGet("GetLibraryGamesByUserId/{userId:int}")]
        public Task<IActionResult> GetByUserId(int userId)
        {
            logger.LogInformation("GET - List library games by user ID: {UserId}", userId);
            return TryMethodAsync(() => service.GetGamesByUserId(userId), logger);
        }
    }
}
