using FCG.Catalog.Application.Interfaces;
using FCG.Core.Integration;
using MassTransit;

namespace FCG.Catalog.Application.Producers;

public class OrderPlacedEventProducer(ISendEndpointProvider sendEndpointProvider) : IOrderPlacedEventProducer
{
	public async Task Send(OrderPlacedEvent message)
    {
        var endpoint = await sendEndpointProvider
            .GetSendEndpoint(new Uri("queue:OrderPlacedEvent"));

        await endpoint.Send(message);
    }
}
