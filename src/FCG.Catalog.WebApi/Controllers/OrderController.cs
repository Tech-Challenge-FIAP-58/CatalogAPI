using FCG.Application.Producers;
using FCG.Application.Services;
using FCG.Catalog.Domain.Inputs;
using FCG.Core.Messages.Integration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.WebApi.Controllers
{
    public class OrderController(IOrderService orderService,
        ILogger<OrderController> logger,
        IOrderPlacedEventProducer orderPlacedEventProducer) : StandardController
    {
		[Authorize(Roles = "Admin")]
        [HttpPost("RegisterOrder")]
        public async Task<IActionResult> Post([FromBody] OrderRegisterDto register)
        {
            logger.LogInformation("POST - Criar Pedido");

            // 1️ grava no banco e PEGA o ID
            var orderId = await orderService.Create(register);
            
            // 2 cria ordem
            register.OrderId = orderId;

			// 3 monta o evento
			OrderPlacedEvent orderRegistered = MapToPayment(register);

            // 4 envia evento
            await orderPlacedEventProducer.Send(orderRegistered);

            return StatusCode(StatusCodes.Status202Accepted);
        }

        private static OrderPlacedEvent MapToPayment(OrderRegisterDto message)
        {
            return new OrderPlacedEvent(
                    message.UserId,
                    message.OrderId,
                    1,
                    message.Price,
                    message.CardName,
                    message.CardNumber,
                    message.ExpirationDate,
                    message.Cvv
                );
        }
    }
}
