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

            // 1️ grava no banco e PEGA o ID
            var orderId = await orderService.Create(register);

            // 2️ monta o evento
            OrderRegisteredDto orderRegistered = new OrderRegisteredDto();
            orderRegistered.OrderId = orderId;
            orderRegistered.UserId = register.UserId;
            orderRegistered.GameId = register.GameId;
            orderRegistered.Price = register.Price;
            orderRegistered.PaymentStatus = register.PaymentStatus;
            orderRegistered.CardName = register.CardName;
            orderRegistered.CardNumber = register.CardNumber;
            orderRegistered.ExpirationDate = register.ExpirationDate;
            orderRegistered.Cvv = register.Cvv;
            
            // 3️ envia para o RabbitMQ
            await rabbitMQServiceProducer.SendMessageAsyncObjeto(orderRegistered);

            return StatusCode(StatusCodes.Status202Accepted);
        }
        /*
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
        }*/

    }
}
