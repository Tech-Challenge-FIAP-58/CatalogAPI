using FCG.Catalog.Application.Interfaces;
using FCG.Catalog.Infra.Mapping;
using FCG.Catalog.Infra.Mongo;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class SearchController(IGameSearchService _search) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<GameDocument>), 200)]
    public async Task<IActionResult> Search(
        [FromQuery] string q,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest("Parâmetro 'q' é obrigatório.");

        var results = await _search.Search(q, page, pageSize);
        return Ok(results);
    }
}