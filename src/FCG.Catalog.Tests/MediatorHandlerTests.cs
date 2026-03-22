using FCG.Catalog.Application.Mediator;
using FCG.Catalog.Domain.Mediatr;
using FluentValidation.Results;
using MediatR;
using Moq;

namespace FCG.Catalog.Tests;

public class MediatorHandlerTests
{
    [Fact]
    public async Task SendCommand_ShouldDelegateToMediator()
    {
        var mediatorMock = new Mock<IMediator>();
        var command = new FakeCommand();
        var expected = new ValidationResult();

        mediatorMock
            .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var sut = new FCG.Catalog.Application.Mediator.MediatorHandler(mediatorMock.Object);

        var result = await sut.SendCommand(command);

        Assert.Equal(expected, result);
        mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task PublishEvent_ShouldDelegateToMediator()
    {
        var mediatorMock = new Mock<IMediator>();
        var evt = new FakeEvent();
        var sut = new FCG.Catalog.Application.Mediator.MediatorHandler(mediatorMock.Object);

        await sut.PublishEvent(evt);

        mediatorMock.Verify(m => m.Publish(evt, It.IsAny<CancellationToken>()), Times.Once);
    }

    private sealed class FakeCommand : Command
    {
    }

    private sealed class FakeEvent : Event
    {
    }
}
