using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;

namespace FCG.Catalog.WebApi.Controllers;

[ApiController]
[Route("redis-test")]
public class RedisTestController(IDistributedCache cache) : ControllerBase
{
    [HttpPost("{key}")]
    public async Task<IActionResult> Set(string key, [FromBody] string value)
    {
        await cache.SetAsync(key, Encoding.UTF8.GetBytes(value), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        });

        return Ok(new { key, value, expiresInMinutes = 5 });
    }

    [HttpGet("{key}")]
    public async Task<IActionResult> Get(string key)
    {
        var bytes = await cache.GetAsync(key);

        if (bytes is null)
            return NotFound(new { key, message = "Chave não encontrada ou expirada." });

        return Ok(new { key, value = Encoding.UTF8.GetString(bytes) });
    }

    [HttpDelete("{key}")]
    public async Task<IActionResult> Delete(string key)
    {
        await cache.RemoveAsync(key);
        return Ok(new { key, message = "Chave removida." });
    }
}
