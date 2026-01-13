using FIAP.FCG.CATALOG.Application.Services;
using FIAP.FCG.CATALOG.Core.Inputs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.FCG.CATALOG.WebApi.Controllers
{
    [Authorize]
    public class OrderController(IOrderService orderService, ILogger<OrderController> logger, IRabbitMQServiceProducer rabbitMQServiceProducer) : StandardController
    {
        //[Authorize(Roles = "Admin")]
        [HttpPost("RegisterOrder")]
        [AllowAnonymous]
        public async Task<IActionResult> Post([FromBody] OrderRegisterDto register)
        {
            logger.LogInformation("POST - Criar Pedido");

            // grava pedido no banco
            TryMethodAsync(() => orderService.Create(register), logger); 

            // envia a mensagem para o RabbitMQ
            await rabbitMQServiceProducer.SendMessageAsyncObjeto(register);

            return StatusCode(StatusCodes.Status202Accepted);
        }

    }
}
