using FCG.Catalog.Application.Interfaces;
using FCG.Core.Integration;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FCG.Catalog.Application.Consumers
{
    public class PaymentProcessedEventConsumer(
        ILogger<PaymentProcessedEventConsumer> logger,
        IOrderService orderService,
        IGameLibraryService gameLibraryService) : IConsumer<PaymentProcessedEvent>
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

                var orderResponse = await orderService.GetById(context.Message.OrderId);

                if (!orderResponse.IsSuccess || orderResponse.ResultValue is null)
                {
                    logger.LogWarning("Failed to load order #{OrderId} to update user library: {Message}", context.Message.OrderId, orderResponse.Message);
                    return;
                }

                var libraryResponse = await gameLibraryService.AddGames(orderResponse.ResultValue.UserId, orderResponse.ResultValue.Items);

                if (!libraryResponse.IsSuccess)
                {
                    logger.LogWarning("Failed to add order games to user library. Order #{OrderId}: {Message}", context.Message.OrderId, libraryResponse.Message);
                    return;
                }

                logger.LogInformation("Order #{OrderId} games were added to user #{UserId} library", context.Message.OrderId, orderResponse.ResultValue.UserId);
            }
            else
            {
                logger.LogInformation("❌ Payment denied");
            }
        }
    }
}
