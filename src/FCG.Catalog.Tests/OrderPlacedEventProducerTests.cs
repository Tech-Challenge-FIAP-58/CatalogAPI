using FCG.Catalog.Application.Producers;
using FCG.Core.Integration;
using MassTransit;
using Moq;

namespace FCG.Catalog.Tests;

public class OrderPlacedEventProducerTests
{
    [Fact]
    public async Task Send_ShouldResolveOrderPlacedQueueAndSendMessage()
    {
        var providerMock = new Mock<ISendEndpointProvider>();
        var endpointMock = new Mock<ISendEndpoint>();
        var message = new OrderPlacedEvent(10, "test@gmail.com", Guid.NewGuid(), PaymentMethod.CreditCard, 150, "John", "123456789012", "12/30", "123");

        providerMock
            .Setup(p => p.GetSendEndpoint(It.Is<Uri>(u => u.OriginalString == "queue:OrderPlacedEvent")))
            .ReturnsAsync(endpointMock.Object);

        var sut = new OrderPlacedEventProducer(providerMock.Object);

        await sut.Send(message);

        providerMock.Verify(p => p.GetSendEndpoint(It.Is<Uri>(u => u.OriginalString == "queue:OrderPlacedEvent")), Times.Once);
        endpointMock.Verify(e => e.Send(message, It.IsAny<CancellationToken>()), Times.Once);
    }
}
