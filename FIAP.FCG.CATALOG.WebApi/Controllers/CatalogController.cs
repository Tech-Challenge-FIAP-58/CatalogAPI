
using FIAP.FCG.CATALOG.Application.Services;
using FIAP.FCG.CATALOG.Core.Inputs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FIAP.FCG.CATALOG.WebApi.Controllers
{
    [AllowAnonymous]
	public class CatalogController(ICatalogService service, ILogger<CatalogController> logger, IRabbitMQServiceProducer rabbitMQServiceProducer) : StandardController
	{
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] CatalogRegisterDto catalogRegisterRequestDto)
        {


            // envia a mensagem para o RabbitMQ
            await rabbitMQServiceProducer.SendMessageAsyncObjeto(catalogRegisterRequestDto);

            //return StatusCode((int)response.StatusCode);
            return StatusCode(StatusCodes.Status202Accepted);
        }
    }

}
