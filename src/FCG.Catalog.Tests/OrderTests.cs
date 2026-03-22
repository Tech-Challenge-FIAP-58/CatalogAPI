using FCG.Catalog.Application.Services;
using FCG.Catalog.Application.Interfaces;
using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Events;
using FCG.Catalog.Domain.Models.Catalog;
using OrderAggregate = FCG.Catalog.Domain.Models.Order.Order;
using FCG.Catalog.Infra.Repository;
using AutoMapper;
using Moq;
using System.Net;

namespace FCG.Catalog.Tests
{
	public class OrderTests
	{
        private readonly Mock<IOrderRepository> _repositoryMock;
     private readonly Mock<IGameRepository> _gameRepositoryMock;
     private readonly Mock<IOrderPlacedEventProducer> _orderPlacedEventProducerMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly OrderService _sut;

		public OrderTests()
		{
         _repositoryMock = new Mock<IOrderRepository>();
          _gameRepositoryMock = new Mock<IGameRepository>();
          _orderPlacedEventProducerMock = new Mock<IOrderPlacedEventProducer>();
			_mapperMock = new Mock<IMapper>();

            _sut = new OrderService(_repositoryMock.Object, _gameRepositoryMock.Object, _orderPlacedEventProducerMock.Object, _mapperMock.Object);
		}

		[Fact]
		public async Task GetOrderByIdTest()
		{
			// Arrange
			var orderId = Guid.NewGuid();
			var userId = 123;
            var gameId = Guid.NewGuid();
            var itemSnapshot = new OrderItemSnapshot(
				gameId,
				"Game",
				"Platform",
				"Publisher",
				"Description",
				150.00M);
         var order = OrderAggregate.Create(DateTime.UtcNow, userId, [itemSnapshot]);
			var orderResponse = new OrderResponseDto(
				order.Id,
				order.OrderDate,
				order.UserId,
				order.Total,
				order.Status,
				order.Items.Select(item => item.ToSnapshot()).ToList());

			_repositoryMock.Setup(r => r.GetById(orderId)).ReturnsAsync(order);
			_mapperMock.Setup(m => m.Map<OrderResponseDto>(order)).Returns(orderResponse);

			// Act
			var response = await _sut.GetById(orderId);

			// Assert
			Assert.NotNull(response);
            Assert.NotNull(response.ResultValue);
			Assert.Equal(order.Id, response.ResultValue!.Id);
			Assert.Equal(123, response.ResultValue.UserId);
			Assert.Equal(150.00M, response.ResultValue.Total);
		}

		[Fact]
		public async Task UpdateOrderTest()
		{
			// Arrange
			var orderId = Guid.NewGuid();
          var itemSnapshot = new OrderItemSnapshot(
				Guid.NewGuid(),
				"Game",
				"Platform",
				"Publisher",
				"Description",
				150.00M);
			var updateDto = new OrderUpdateDto
			{
				OrderDate = DateTime.UtcNow,
				UserId = 123,
				Total = 150.00M,
				OrderGames =
				[
                 new OrderItemRegisterDto
					{
                        GameId = itemSnapshot.GameId
					}
				]
			};
          var order = OrderAggregate.Create(updateDto.OrderDate, updateDto.UserId, [itemSnapshot]);
			_repositoryMock.Setup(r => r.GetById(orderId)).ReturnsAsync(order);
            var game = Game.Create(itemSnapshot.Name, itemSnapshot.Platform, itemSnapshot.PublisherName, itemSnapshot.Description, itemSnapshot.Price);
			game = Game.Rehydrate(itemSnapshot.GameId, game.Name, game.Platform, game.PublisherName, game.Description, game.Price, true, DateTime.UtcNow);
			_gameRepositoryMock.Setup(r => r.GetById(itemSnapshot.GameId)).ReturnsAsync(game);
         _repositoryMock.Setup(r => r.Update(orderId, order));

			// Act
			var response = await _sut.Update(orderId, updateDto);

			// Assert
			Assert.True(response.IsSuccess);
			Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
		}

		[Fact]
		public async Task CreateOrderTest()
		{
			var id = Guid.NewGuid();

            // Arrange
         var orderRegisterDto = new OrderRegisterDto
			{
				OrderDate = DateTime.UtcNow,
				UserId = 123,
				OrderGames =
				[
                 new OrderItemRegisterDto
					{
                        GameId = Guid.NewGuid()
					}
				]
			};

			var game = Game.Create("Game", "Platform", "Publisher", "Description", 150.00M);
			game = Game.Rehydrate(orderRegisterDto.OrderGames[0].GameId, game.Name, game.Platform, game.PublisherName, game.Description, game.Price, true, DateTime.UtcNow);

			_gameRepositoryMock.Setup(r => r.GetById(orderRegisterDto.OrderGames[0].GameId)).ReturnsAsync(game);

          _repositoryMock.Setup(r => r.Create(It.IsAny<OrderAggregate>())).Returns(id);

			// Act
			var response = await _sut.Create(orderRegisterDto);

			// Assert
			Assert.True(response.IsSuccess);
           Assert.Equal(HttpStatusCode.Created, response.StatusCode);
			Assert.Equal(id, response.ResultValue);
           Assert.Equal("Order created successfully.", response.Message);
		}
	}
}
