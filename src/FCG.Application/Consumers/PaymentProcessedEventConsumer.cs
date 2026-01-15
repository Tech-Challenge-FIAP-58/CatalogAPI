using FCG.Core.Messages.Integration;
using MassTransit;

namespace FCG.Application.Consumers
{
    public class PaymentProcessedEventConsumer : IConsumer<PaymentProcessedEvent>
    {

        public PaymentProcessedEventConsumer()
        {
        }

        public async Task Consume(
            ConsumeContext<PaymentProcessedEvent> context)
        {
            Console.WriteLine($"Payment processed for OrderId: {context.Message.OrderId}, PaymentId: {context.Message.PaymentId}, Amount: {context.Message.Amount}, Status: {context.Message.Status}, Reason: {context.Message.Reason}");
        }

    }
}
