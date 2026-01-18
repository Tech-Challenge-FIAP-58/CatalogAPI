using FCG.Core.Contracts;
using MassTransit;

namespace FCG.Application.Producers;

public interface IOrderPlacedEventProducer
{
    Task Send(OrderPlacedEvent message);
}

public class OrderPlacedEventProducer : IOrderPlacedEventProducer
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public OrderPlacedEventProducer(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }

    public async Task Send(OrderPlacedEvent message)
    {
        var endpoint = await _sendEndpointProvider
            .GetSendEndpoint(new Uri("queue:OrderPlacedEvent"));

        await endpoint.Send(message);
    }
}
