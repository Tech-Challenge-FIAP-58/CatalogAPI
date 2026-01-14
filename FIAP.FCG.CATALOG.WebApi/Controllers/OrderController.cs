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
