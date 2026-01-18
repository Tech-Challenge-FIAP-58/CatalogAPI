using FCG.Core.Core.Inputs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.WebApi.Controllers
{
    public class CatalogController() : StandardController
    {
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] CatalogRegisterDto catalogRegisterRequestDto)
        {
            // envia a mensagem para o RabbitMQ

            //return StatusCode((int)response.StatusCode);
            return StatusCode(StatusCodes.Status202Accepted);
        }
    }

}
