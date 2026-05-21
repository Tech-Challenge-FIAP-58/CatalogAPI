using FCG.Catalog.Application.Interfaces;
using FCG.Catalog.Infra.Repository;
using FCG.Core;
using FCG.Core.Integration;
using MassTransit;

namespace FCG.Catalog.Application.Producers;

public class OrderPlacedEventProducer(ISendEndpointProvider sendEndpointProvider, IEventLogRepository eventLogRepository) : IOrderPlacedEventProducer
{
	public async Task Send(OrderPlacedEvent message)
    {
        var endpoint = await sendEndpointProvider
            .GetSendEndpoint(new Uri("queue:OrderPlacedEvent"));

        await endpoint.Send(message);
        await eventLogRepository.InsertOrderPlacedEventLog(new OrderPlacedEventLog(
            message.ClientId,
            message.OrderId,
            message.PaymentMethod,
            message.Amount,
            message.CardName,
            message.CardNumber,
            message.ExpirationDate,
            message.Cvv
        )
        {
            Message = "Ordem criada",
            DtCpu = DateTime.Now
        });
    }
}
