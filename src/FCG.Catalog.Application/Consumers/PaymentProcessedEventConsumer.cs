using FCG.Catalog.Application.Interfaces;
using FCG.Core.Integration;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FCG.Catalog.Application.Consumers
{
    public class PaymentProcessedEventConsumer(ILogger<PaymentProcessedEventConsumer> logger, IOrderService orderService) : IConsumer<PaymentProcessedEvent>
    {
        public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
        {
            logger.LogInformation("Payment processed for order #{OrderId}", context.Message.OrderId);

            var updateResponse = await orderService.UpdatePaymentStatus(context.Message.OrderId, context.Message.Status);

            if (!updateResponse.IsSuccess)
            {
                logger.LogWarning("Failed to update payment status for order #{OrderId}: {Message}", context.Message.OrderId, updateResponse.Message);
                return;
            }

            if (context.Message.Status == PaymentResultStatus.Approved)
            {
                logger.LogInformation("✅ Payment approved");
            }
            else
            {
                logger.LogInformation("❌ Payment denied");
            }
        }
    }
}
