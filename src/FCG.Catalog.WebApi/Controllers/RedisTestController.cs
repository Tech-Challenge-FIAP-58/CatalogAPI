using FCG.Catalog.Infra.Caching;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Catalog.WebApi.Controllers;

[ApiController]
[Route("redis-test")]
public class RedisTestController(ICachingService cachingService) : ControllerBase
{
    [HttpPost("{key}")]
    public async Task<IActionResult> Set(string key, [FromBody] string value)
    {
        await cachingService.SetAsync(key, value);
        return Ok("Valor definido.");
    }

    [HttpGet("{key}")]
    public async Task<IActionResult> Get(string key)
    {
        var rs = await cachingService.GetAsync<string>(key);
        if (rs == null)
        {
            return Ok("Chave não encontrada ou expirou.");
		}

		return Ok(rs);
    }

    [HttpDelete("{key}")]
    public async Task<IActionResult> Delete(string key)
    {
        await cachingService.RemoveAsync(key);
        return Ok(new { key, message = "Chave removida." });
    }
}
