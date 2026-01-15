using FCG.Core.Messages.Integration;
using FIAP.FCG.CATALOG.Application.Producers;
using FIAP.FCG.CATALOG.Application.Services;
using FIAP.FCG.CATALOG.Core.Inputs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.FCG.CATALOG.WebApi.Controllers
{
    [Authorize]
    public class OrderController(IOrderService orderService, 
        ILogger<OrderController> logger, 
        IRabbitMQServiceProducer rabbitMQServiceProducer,
        IOrderPlacedEventProducer orderPlacedEventProducer) : StandardController
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
            OrderPlacedEvent orderRegistered = MapToPayment(register);

            // 3️ envia para o RabbitMQ
            //await rabbitMQServiceProducer.SendMessageAsyncObjeto(orderRegistered);
            await orderPlacedEventProducer.Send(orderRegistered);

            return StatusCode(StatusCodes.Status202Accepted);
        }

        private static OrderPlacedEvent MapToPayment(OrderRegisterDto message)
        {
            return new OrderPlacedEvent(
                    message.UserId,
                    message.GameId,
                    1,
                    message.Price,
                    message.CardName,
                    message.CardNumber,
                    message.ExpirationDate,
                    message.Cvv
                );
        }
        /*
        [HttpGet("GetOrderById/{id:int}")]
        [AllowAnonymous]
        public Task<IActionResult> GetById(int id)
        {
            logger.LogInformation("GET - Listar pedido por ID: {Id}", id);
            return TryMethodAsync(() => orderService.GetById(id), logger);
        }

        [HttpGet("GetOrderById{id}")]
        [AllowAnonymous]
        public Task<IActionResult> GetById(int id)
        {

            try
            {
                logger.LogInformation("GET - Listar pedido por ID: {Id}", id);
                return TryMethodAsync(() => orderService.GetById(id), logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao listar pedido por ID: {Id}", id);
                throw;
            }
        }

        //[Authorize(Roles = "Admin")]
        [HttpPut("UpdateOrder/{id:int}")]
        [AllowAnonymous]
        public Task<IActionResult> Put(int id, [FromBody] OrderUpdateDto update)
        {
            logger.LogInformation("PUT - Atualizar pedido com ID: {Id}", id);
            return TryMethodAsync(() => orderService.Update(id, update), logger);
        }*/

    }
}
