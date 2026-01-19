using FCG.Catalog.Application.Interfaces;
using FCG.Core.Messages.Integration;
using MassTransit;

namespace FCG.Catalog.Application.Producers;

public class OrderPlacedEventProducer(ISendEndpointProvider sendEndpointProvider) : IOrderPlacedEventProducer
{
    private readonly ISendEndpointProvider _sendEndpointProvider = sendEndpointProvider;

	public async Task Send(OrderPlacedEvent message)
    {
        var endpoint = await _sendEndpointProvider
            .GetSendEndpoint(new Uri("queue:OrderPlacedEvent"));

        await endpoint.Send(message);
    }
}
