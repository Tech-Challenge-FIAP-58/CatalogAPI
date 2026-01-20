using FCG.Catalog.Application.Interfaces;
using FCG.Catalog.Domain.Inputs;
using FCG.Core.Integration;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FCG.Catalog.Application.Consumers
{
    public class PaymentProcessedEventConsumer(ILogger<PaymentProcessedEventConsumer> logger, IOrderService orderService, ICatalogService catalogService) : IConsumer<PaymentProcessedEvent>
    {
		public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
        {
            logger.LogInformation("Pagamento processo para ordem #{}", context.Message.OrderId);

            var order = await orderService.GetById(context.Message.OrderId) 
                ?? throw new NullReferenceException("Ordem não encontrada");

			var status = context.Message.Status == PaymentResultStatus.Approved
                ? "Approved"
                : "Denied";

            var orderUpdate = new OrderUpdateDto
            {
                OrderDate = order.OrderDate,
                UserId = order.UserId,
                GameId = order.GameId,
                Price = order.Price,
                PaymentStatus = status,
                CardName = order.CardName,
                CardNumber = order.CardNumber,
                ExpirationDate = order.ExpirationDate,
                Cvv = order.Cvv
            };

            await orderService.Update(context.Message.OrderId, orderUpdate);

            if (context.Message.Status == PaymentResultStatus.Approved)
            {
                var catalogRegister = new CatalogRegisterDto
                {
                    UserId = order.UserId,
                    GameId = order.GameId,
                    Price = order.Price
                };

                await catalogService.Create(catalogRegister);
				logger.LogInformation("✅ Pagamento aprovado, jogo adicionado ao catálogo");
            }
            else
            {
                logger.LogInformation("❌ Pagamento negado");
            }
        }
    }
}
