using FCG.Catalog.Application.Interfaces;
using FCG.Catalog.Application.Services;
using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Infra.Repository;
using FCG.Core.Messages.Integration;
using Moq;
using System.Net;

namespace FCG.Catalog.Tests
{
	public class OrderTests
	{
		private readonly Mock<IOrderRepository> _repositoryMock;
		private readonly Mock<IOrderPlacedEventProducer> _eventProducerMock;
		private readonly OrderService _sut;

		public OrderTests()
		{
			_repositoryMock = new Mock<IOrderRepository>();
			_eventProducerMock = new Mock<IOrderPlacedEventProducer>();

			_sut = new OrderService(_repositoryMock.Object, _eventProducerMock.Object);
		}

		[Fact]
		public async Task GetOrderByIdTest()
		{
			// Arrange
			var orderId = 1;
			var userId = 123;
			var gameId = 1;

			_repositoryMock.Setup(r => r.GetById(orderId)).ReturnsAsync(new OrderResponseDto
			(
				orderId,
				DateTime.Now,
				userId,
				gameId,
				150.00M,
				"Pending",
				"John Doe",
				"4111111111111111",
				"12/25",
				"123"
			));

			// Act
			var response = await _sut.GetById(orderId);

			// Assert
			Assert.NotNull(response);
			Assert.Equal(orderId, response!.Id);
			Assert.Equal(123, response.UserId);
			Assert.Equal(150.00M, response.Price);
			Assert.Equal("Pending", response.PaymentStatus);
		}

		[Fact]
		public async Task UpdateOrderTest()
		{
			// Arrange
			var orderId = 1;
			var updateDto = new OrderUpdateDto
			{
				PaymentStatus = "Completed"
			};
			_repositoryMock.Setup(r => r.Update(orderId, updateDto)).ReturnsAsync(true);

			// Act
			var response = await _sut.Update(orderId, updateDto);

			// Assert
			Assert.True(response.IsSuccess);
			Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
		}

		[Fact]
		public async Task CreateOrderTest()
		{
			// Arrange
			var orderRegisterDto = new OrderRegisterDto
			{
				OrderDate = DateTime.Now,
				UserId = 123,
				Price = 150.00M,
				PaymentStatus = "Pending",
				CardName = "John Doe",
				CardNumber = "4111111111111111",
				ExpirationDate = "12/25",
				Cvv = "123"
			};
			_repositoryMock.Setup(r => r.Create(orderRegisterDto)).ReturnsAsync(1);

			// Act
			var response = await _sut.Create(orderRegisterDto);

			// Assert
			Assert.True(response.IsSuccess);
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			Assert.Equal(1, response.ResultValue);
			Assert.Equal("Ordem #1 criada com sucesso", response.Message);

			_eventProducerMock.Verify(ep => ep.Send(It.IsAny<OrderPlacedEvent>()), Times.Once);
		}
	}
}
