using FCG.Catalog.Application.Consumers;
using FCG.Catalog.Application.Interfaces;
using FCG.Catalog.Domain.Events;
using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Web;
using FCG.Core.Integration;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;

namespace FCG.Catalog.Tests;

public class PaymentProcessedEventConsumerTests
{
    private readonly Mock<IOrderPaymentProcessingService> _orderPaymentProcessingServiceMock;
    private readonly Mock<IGameLibraryOwnershipService> _gameLibraryOwnershipServiceMock;
    private readonly Mock<ILogger<PaymentProcessedEventConsumer>> _loggerMock;
    private readonly PaymentProcessedEventConsumer _sut;

    public PaymentProcessedEventConsumerTests()
    {
        _orderPaymentProcessingServiceMock = new Mock<IOrderPaymentProcessingService>();
        _gameLibraryOwnershipServiceMock = new Mock<IGameLibraryOwnershipService>();
        _loggerMock = new Mock<ILogger<PaymentProcessedEventConsumer>>();

        _sut = new PaymentProcessedEventConsumer(_loggerMock.Object, _orderPaymentProcessingServiceMock.Object, _gameLibraryOwnershipServiceMock.Object);
    }

    [Fact]
    public async Task Consume_ShouldStopFlow_WhenUpdatePaymentStatusFails()
    {
        var evt = new PaymentProcessedEvent(Guid.NewGuid(), Guid.NewGuid(), 100, PaymentResultStatus.Approved);
        var context = BuildContext(evt);
        var updateFailure = new TestApiResponse<bool>
        {
            IsSuccess = false,
            StatusCode = HttpStatusCode.NotFound,
            Message = "Order not found."
        };

        _orderPaymentProcessingServiceMock.Setup(s => s.UpdatePaymentStatus(evt.OrderId, evt.Status)).ReturnsAsync(updateFailure);

        await _sut.Consume(context.Object);

        _orderPaymentProcessingServiceMock.Verify(s => s.GetById(It.IsAny<Guid>()), Times.Never);
        _gameLibraryOwnershipServiceMock.Verify(s => s.AddGames(It.IsAny<int>(), It.IsAny<IReadOnlyCollection<OrderItemSnapshot>>()), Times.Never);
    }

    [Fact]
    public async Task Consume_ShouldNotAddGames_WhenPaymentIsDenied()
    {
        var evt = new PaymentProcessedEvent(Guid.NewGuid(), Guid.NewGuid(), 100, PaymentResultStatus.Denied);
        var context = BuildContext(evt);
        var updateSuccess = new TestApiResponse<bool> { IsSuccess = true, StatusCode = HttpStatusCode.NoContent };

        _orderPaymentProcessingServiceMock.Setup(s => s.UpdatePaymentStatus(evt.OrderId, evt.Status)).ReturnsAsync(updateSuccess);

        await _sut.Consume(context.Object);

        _orderPaymentProcessingServiceMock.Verify(s => s.GetById(It.IsAny<Guid>()), Times.Never);
        _gameLibraryOwnershipServiceMock.Verify(s => s.AddGames(It.IsAny<int>(), It.IsAny<IReadOnlyCollection<OrderItemSnapshot>>()), Times.Never);
    }

    [Fact]
    public async Task Consume_ShouldStopFlow_WhenApprovedButOrderLoadFails()
    {
        var evt = new PaymentProcessedEvent(Guid.NewGuid(), Guid.NewGuid(), 100, PaymentResultStatus.Approved);
        var context = BuildContext(evt);
        var updateSuccess = new TestApiResponse<bool> { IsSuccess = true, StatusCode = HttpStatusCode.NoContent };
        var orderFailure = new TestApiResponse<OrderResponseDto?>
        {
            IsSuccess = false,
            StatusCode = HttpStatusCode.NotFound,
            Message = "Order not found.",
            ResultValue = null
        };

        _orderPaymentProcessingServiceMock.Setup(s => s.UpdatePaymentStatus(evt.OrderId, evt.Status)).ReturnsAsync(updateSuccess);
        _orderPaymentProcessingServiceMock.Setup(s => s.GetById(evt.OrderId)).ReturnsAsync(orderFailure);

        await _sut.Consume(context.Object);

        _gameLibraryOwnershipServiceMock.Verify(s => s.AddGames(It.IsAny<int>(), It.IsAny<IReadOnlyCollection<OrderItemSnapshot>>()), Times.Never);
    }

    [Fact]
    public async Task Consume_ShouldCallAddGames_WhenApprovedAndOrderIsLoaded()
    {
        var evt = new PaymentProcessedEvent(Guid.NewGuid(), Guid.NewGuid(), 100, PaymentResultStatus.Approved);
        var context = BuildContext(evt);
        var updateSuccess = new TestApiResponse<bool> { IsSuccess = true, StatusCode = HttpStatusCode.NoContent };
        var orderResponse = BuildOrderResponse(evt.OrderId, 99);
        var orderSuccess = new TestApiResponse<OrderResponseDto?>
        {
            IsSuccess = true,
            StatusCode = HttpStatusCode.OK,
            ResultValue = orderResponse
        };
        var librarySuccess = new TestApiResponse<bool> { IsSuccess = true, StatusCode = HttpStatusCode.NoContent };

        _orderPaymentProcessingServiceMock.Setup(s => s.UpdatePaymentStatus(evt.OrderId, evt.Status)).ReturnsAsync(updateSuccess);
        _orderPaymentProcessingServiceMock.Setup(s => s.GetById(evt.OrderId)).ReturnsAsync(orderSuccess);
        _gameLibraryOwnershipServiceMock.Setup(s => s.AddGames(orderResponse.UserId, orderResponse.Items)).ReturnsAsync(librarySuccess);

        await _sut.Consume(context.Object);

        _gameLibraryOwnershipServiceMock.Verify(s => s.AddGames(orderResponse.UserId, orderResponse.Items), Times.Once);
    }

    [Fact]
    public async Task Consume_ShouldStillAttemptAddGames_WhenApprovedAndLibraryUpdateFails()
    {
        var evt = new PaymentProcessedEvent(Guid.NewGuid(), Guid.NewGuid(), 100, PaymentResultStatus.Approved);
        var context = BuildContext(evt);
        var updateSuccess = new TestApiResponse<bool> { IsSuccess = true, StatusCode = HttpStatusCode.NoContent };
        var orderResponse = BuildOrderResponse(evt.OrderId, 99);
        var orderSuccess = new TestApiResponse<OrderResponseDto?>
        {
            IsSuccess = true,
            StatusCode = HttpStatusCode.OK,
            ResultValue = orderResponse
        };
        var libraryFailure = new TestApiResponse<bool>
        {
            IsSuccess = false,
            StatusCode = HttpStatusCode.BadRequest,
            Message = "Library update failed."
        };

        _orderPaymentProcessingServiceMock.Setup(s => s.UpdatePaymentStatus(evt.OrderId, evt.Status)).ReturnsAsync(updateSuccess);
        _orderPaymentProcessingServiceMock.Setup(s => s.GetById(evt.OrderId)).ReturnsAsync(orderSuccess);
        _gameLibraryOwnershipServiceMock.Setup(s => s.AddGames(orderResponse.UserId, orderResponse.Items)).ReturnsAsync(libraryFailure);

        await _sut.Consume(context.Object);

        _gameLibraryOwnershipServiceMock.Verify(s => s.AddGames(orderResponse.UserId, orderResponse.Items), Times.Once);
    }

    private static Mock<ConsumeContext<PaymentProcessedEvent>> BuildContext(PaymentProcessedEvent evt)
    {
        var context = new Mock<ConsumeContext<PaymentProcessedEvent>>();
        context.SetupGet(c => c.Message).Returns(evt);
        return context;
    }

    private static OrderResponseDto BuildOrderResponse(Guid orderId, int userId)
    {
        IReadOnlyCollection<OrderItemSnapshot> items =
        [
            new(Guid.NewGuid(), "GTA", "PC", "Rockstar", "Desc", 100)
        ];

        return new OrderResponseDto(orderId, DateTime.UtcNow, userId, 100, FCG.Catalog.Domain.Enums.OrderStatus.Paid, items);
    }

    private sealed class TestApiResponse<T> : IApiResponse<T>
    {
        public T? ResultValue { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
    }
}
