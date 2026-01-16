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

        public PaymentProcessedEventConsumer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
        {
            Console.WriteLine($"Payment processed for OrderId: {context.Message.OrderId}, PaymentId: {context.Message.PaymentId}, Amount: {context.Message.Amount}, Status: {context.Message.Status}, Reason: {context.Message.Reason}");


            using var scope = _scopeFactory.CreateScope();
            var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
            var catalogService = scope.ServiceProvider.GetRequiredService<ICatalogService>();

            var order = await orderService.GetById(context.Message.OrderId);

            OrderUpdateDto orderUpdate = new OrderUpdateDto();

            string status = "";

            if (context.Message.Status == PaymentResultStatus.Approved)
                status = "Approved";
            else
                status = "Denied";

            
            orderUpdate.OrderDate = order.OrderDate;
            orderUpdate.UserId = order.UserId;
            orderUpdate.GameId = order.GameId;
            orderUpdate.Price = order.Price;
            orderUpdate.PaymentStatus = status; // pega o paymentStatus que veio na mensagem do rabbitMQ
            orderUpdate.CardName = order.CardName;
            orderUpdate.CardNumber = order.CardNumber;
            orderUpdate.ExpirationDate = order.ExpirationDate;
            orderUpdate.Cvv = order.Cvv;

            // atualiza o pedido com o status do pagamento
            await orderService.Update(context.Message.OrderId, orderUpdate);

            CatalogRegisterDto catalogRegisterDto = new CatalogRegisterDto();
            catalogRegisterDto.UserId = order.UserId;
            catalogRegisterDto.GameId = order.GameId;
            catalogRegisterDto.Price = order.Price;

            if (context.Message.Status == PaymentResultStatus.Approved) // Approved
            {
                // insere o jogo no cadastro do cliente
                await catalogService.Create(catalogRegisterDto);
                Console.WriteLine("✅ Retorno do pagamento processado com sucesso!\n");
            }
            else
            {
                Console.WriteLine("✅ Pagamento " + status + "\n");
            }
        }

    }
}
