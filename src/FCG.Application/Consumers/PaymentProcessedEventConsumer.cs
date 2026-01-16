using FCG.Application.Services;
using FCG.Core.Core.Inputs;
using FCG.Core.Messages.Integration;
using MassTransit;
using MassTransit.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FCG.Application.Consumers
{
    public class PaymentProcessedEventConsumer : IConsumer<PaymentProcessedEvent>
    {

        private readonly IServiceScopeFactory _scopeFactory;

        private readonly IOrderService _orderService;
        private readonly ICatalogService _catalogService;

        public PaymentProcessedEventConsumer(IOrderService orderService,ICatalogService catalogService)
        {
            _orderService = orderService;
            _catalogService = catalogService;
        }

        public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
        {
            Console.WriteLine(
                $"Payment processed for OrderId: {context.Message.OrderId}"
            );

            var order = await _orderService.GetById(context.Message.OrderId);

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

            await _orderService.Update(context.Message.OrderId, orderUpdate);

            if (context.Message.Status == PaymentResultStatus.Approved)
            {
                var catalogRegister = new CatalogRegisterDto
                {
                    UserId = order.UserId,
                    GameId = order.GameId,
                    Price = order.Price
                };

                await _catalogService.Create(catalogRegister);
                Console.WriteLine("✅ Pagamento aprovado, jogo adicionado ao catálogo");
            }
            else
            {
                Console.WriteLine("❌ Pagamento negado");
            }
        }

    }
}
