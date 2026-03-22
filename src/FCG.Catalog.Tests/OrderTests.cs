using AutoMapper;
using FCG.Catalog.Application.Interfaces;
using FCG.Catalog.Application.Services;
using FCG.Catalog.Domain.Enums;
using FCG.Catalog.Domain.Events;
using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Models.Catalog;
using FCG.Catalog.Infra.Repository;
using FCG.Core.Integration;
using Moq;
using OrderAggregate = FCG.Catalog.Domain.Models.Order.Order;
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
        public async Task GetById_ShouldReturnMappedOrder_WhenOrderExists()
		{
			var orderId = Guid.NewGuid();
           var userId = 123;
			var gameId = Guid.NewGuid();
			var itemSnapshot = new OrderItemSnapshot(gameId, "Game", "Platform", "Publisher", "Description", 150.00M);
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

			var response = await _sut.GetById(orderId);

			Assert.NotNull(response);
           Assert.NotNull(response.ResultValue);
			Assert.Equal(order.Id, response.ResultValue!.Id);
         Assert.Equal(userId, response.ResultValue.UserId);
			Assert.Equal(150.00M, response.ResultValue.Total);
		}

		[Fact]
		public async Task GetByUserId_ShouldReturnOkWithEmptyCollection_WhenUserHasNoOrders()
		{
			const int userId = 123;
			IReadOnlyCollection<OrderAggregate> orders = [];
			IReadOnlyCollection<OrderResponseDto> mappedOrders = [];

			_repositoryMock.Setup(r => r.GetByUserId(userId)).ReturnsAsync(orders);
			_mapperMock.Setup(m => m.Map<IReadOnlyCollection<OrderResponseDto>>(orders)).Returns(mappedOrders);

			var response = await _sut.GetByUserId(userId);

			Assert.True(response.IsSuccess);
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			Assert.NotNull(response.ResultValue);
			Assert.Empty(response.ResultValue!);
		}

		[Fact]
		public async Task GetById_ShouldReturnNotFound_WhenOrderDoesNotExist()
		{
			var orderId = Guid.NewGuid();
			_repositoryMock.Setup(r => r.GetById(orderId)).ReturnsAsync((OrderAggregate?)null);

			var response = await _sut.GetById(orderId);

			Assert.False(response.IsSuccess);
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
			Assert.Equal("Order not found.", response.Message);
			Assert.Null(response.ResultValue);
		}

		[Fact]
		public async Task GetByUserId_ShouldReturnMappedOrders_WhenOrdersExist()
		{
			const int userId = 123;
			var order = BuildOrderWithSingleItem();
			IReadOnlyCollection<OrderAggregate> orders = [order];
			IReadOnlyCollection<OrderResponseDto> mappedOrders =
			[
				new OrderResponseDto(
					order.Id,
					order.OrderDate,
					order.UserId,
					order.Total,
					order.Status,
					order.Items.Select(item => item.ToSnapshot()).ToList())
			];

			_repositoryMock.Setup(r => r.GetByUserId(userId)).ReturnsAsync(orders);
			_mapperMock.Setup(m => m.Map<IReadOnlyCollection<OrderResponseDto>>(orders)).Returns(mappedOrders);

			var response = await _sut.GetByUserId(userId);

			Assert.True(response.IsSuccess);
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			Assert.NotNull(response.ResultValue);
			Assert.Single(response.ResultValue!);
			Assert.Equal(order.Id, response.ResultValue.First().Id);
		}

		[Fact]
		public async Task Create_ShouldReturnBadRequest_WhenDtoIsInvalid()
		{
			var invalidDto = new OrderRegisterDto
			{
				OrderDate = DateTime.UtcNow,
				UserId = 123,
				OrderGames = []
			};

			var response = await _sut.Create(invalidDto);

			Assert.False(response.IsSuccess);
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			Assert.StartsWith("Invalid order data:", response.Message);
			_repositoryMock.Verify(r => r.Create(It.IsAny<OrderAggregate>()), Times.Never);
		}

		[Fact]
     public async Task Create_ShouldReturnBadRequest_WhenOrderContainsDuplicatedGames()
		{
          var duplicatedGameId = Guid.NewGuid();
			var registerDto = new OrderRegisterDto
			{
				OrderDate = DateTime.UtcNow,
				UserId = 123,
				OrderGames =
				[
                   new OrderItemRegisterDto { GameId = duplicatedGameId },
					new OrderItemRegisterDto { GameId = duplicatedGameId }
				]
			};

          var response = await _sut.Create(registerDto);

           Assert.False(response.IsSuccess);
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			Assert.Equal("Order already contains duplicated games.", response.Message);
			_gameRepositoryMock.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
			_repositoryMock.Verify(r => r.Create(It.IsAny<OrderAggregate>()), Times.Never);
		}

		[Fact]
     public async Task Create_ShouldReturnNotFound_WhenAnyGameDoesNotExist()
		{
            var missingGameId = Guid.NewGuid();
			var registerDto = new OrderRegisterDto
			{
				OrderDate = DateTime.UtcNow,
				UserId = 123,
				OrderGames =
				[
                   new OrderItemRegisterDto { GameId = missingGameId }
				]
			};

            _gameRepositoryMock.Setup(r => r.GetById(missingGameId)).ReturnsAsync((Game?)null);
			var response = await _sut.Create(registerDto);

           Assert.False(response.IsSuccess);
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
			Assert.Equal($"Game not found: {missingGameId}", response.Message);
			_repositoryMock.Verify(r => r.Create(It.IsAny<OrderAggregate>()), Times.Never);
		}

		[Fact]
		public async Task Create_ShouldReturnBadRequest_WhenGameIsNotAvailable()
		{
			var unavailableGameId = Guid.NewGuid();
			var registerDto = BuildValidOrderRegisterDto(unavailableGameId);
			var unavailableGame = BuildGame(unavailableGameId, "Unavailable game", 150.00M, false);

			_gameRepositoryMock.Setup(r => r.GetById(unavailableGameId)).ReturnsAsync(unavailableGame);

			var response = await _sut.Create(registerDto);

			Assert.False(response.IsSuccess);
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			Assert.Equal("Game is not available: Unavailable game", response.Message);
			_repositoryMock.Verify(r => r.Create(It.IsAny<OrderAggregate>()), Times.Never);
		}

		[Fact]
		public async Task Create_ShouldCreateOrderAndPublishEvent_WhenCheckoutDataIsProvided()
		{
			var createdOrderId = Guid.NewGuid();
			var gameId = Guid.NewGuid();
			var registerDto = BuildValidOrderRegisterDto(gameId);
			var checkoutDto = new CheckoutCartDto
			{
				ClientId = 10,
				PaymentMethod = Domain.Inputs.PaymentMethod.CreditCard,
				Amount = 150.00M,
				CardName = "John Doe",
				CardNumber = "123456789012",
				ExpirationDate = "12/30",
				Cvv = "123"
			};
			var game = BuildGame(gameId, "Game", 150.00M, true);

			_gameRepositoryMock.Setup(r => r.GetById(gameId)).ReturnsAsync(game);
			_repositoryMock.Setup(r => r.Create(It.IsAny<OrderAggregate>())).Returns(createdOrderId);

			var response = await _sut.Create(registerDto, checkoutDto);

			Assert.True(response.IsSuccess);
			Assert.Equal(HttpStatusCode.Created, response.StatusCode);
			Assert.Equal(createdOrderId, response.ResultValue);
			Assert.Equal("Order created successfully.", response.Message);
          _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
			_orderPlacedEventProducerMock.Verify(p => p.Send(It.Is<OrderPlacedEvent>(e =>
				e.ClientId == checkoutDto.ClientId &&
				e.OrderId == createdOrderId &&
				e.PaymentMethod == FCG.Core.Integration.PaymentMethod.CreditCard &&
				e.Amount == checkoutDto.Amount &&
				e.CardName == checkoutDto.CardName &&
				e.CardNumber == checkoutDto.CardNumber &&
				e.ExpirationDate == checkoutDto.ExpirationDate &&
				e.Cvv == checkoutDto.Cvv)), Times.Once);
		}

		[Fact]
		public async Task Create_ShouldCreateOrderWithoutPublishingEvent_WhenCheckoutDataIsNotProvided()
		{
			var createdOrderId = Guid.NewGuid();
			var gameId = Guid.NewGuid();
			var registerDto = BuildValidOrderRegisterDto(gameId);
			var game = BuildGame(gameId, "Game", 150.00M, true);

			_gameRepositoryMock.Setup(r => r.GetById(gameId)).ReturnsAsync(game);
			_repositoryMock.Setup(r => r.Create(It.IsAny<OrderAggregate>())).Returns(createdOrderId);

			var response = await _sut.Create(registerDto);

			Assert.True(response.IsSuccess);
			Assert.Equal(HttpStatusCode.Created, response.StatusCode);
			Assert.Equal(createdOrderId, response.ResultValue);
			_repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
			_orderPlacedEventProducerMock.Verify(p => p.Send(It.IsAny<OrderPlacedEvent>()), Times.Never);
		}

		[Fact]
		public async Task Update_ShouldReturnBadRequest_WhenDtoIsInvalid()
		{
			var orderId = Guid.NewGuid();
			var invalidDto = new OrderUpdateDto
			{
				OrderDate = DateTime.UtcNow,
				UserId = 123,
				Total = 100,
				OrderGames = []
			};

			var response = await _sut.Update(orderId, invalidDto);

			Assert.False(response.IsSuccess);
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			Assert.StartsWith("Invalid order data:", response.Message);
			_repositoryMock.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
		}

		[Fact]
		public async Task Update_ShouldReturnNotFound_WhenOrderDoesNotExist()
		{
			var orderId = Guid.NewGuid();
			var gameId = Guid.NewGuid();
			var updateDto = BuildValidOrderUpdateDto(gameId);

			_repositoryMock.Setup(r => r.GetById(orderId)).ReturnsAsync((OrderAggregate?)null);

			var response = await _sut.Update(orderId, updateDto);

			Assert.False(response.IsSuccess);
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
			Assert.Equal("Order not found.", response.Message);
			_gameRepositoryMock.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
		}

		[Fact]
		public async Task Update_ShouldReturnBadRequest_WhenOrderContainsDuplicatedGames()
		{
			var orderId = Guid.NewGuid();
			var duplicatedGameId = Guid.NewGuid();
			var existingOrder = BuildOrderWithSingleItem();
			var updateDto = new OrderUpdateDto
			{
				OrderDate = DateTime.UtcNow,
				UserId = 123,
				Total = 999,
				OrderGames =
				[
					new OrderItemRegisterDto { GameId = duplicatedGameId },
					new OrderItemRegisterDto { GameId = duplicatedGameId }
				]
			};

			_repositoryMock.Setup(r => r.GetById(orderId)).ReturnsAsync(existingOrder);

			var response = await _sut.Update(orderId, updateDto);

			Assert.False(response.IsSuccess);
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			Assert.Equal("Order already contains duplicated games.", response.Message);
			_gameRepositoryMock.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
			_repositoryMock.Verify(r => r.Update(It.IsAny<Guid>(), It.IsAny<OrderAggregate>()), Times.Never);
		}

		[Fact]
		public async Task Update_ShouldRecalculateTotalBasedOnGamesAndPersistOrder()
		{
			var orderId = Guid.NewGuid();
			var firstGameId = Guid.NewGuid();
			var secondGameId = Guid.NewGuid();
			var existingOrder = BuildOrderWithSingleItem();
			var updateDto = new OrderUpdateDto
			{
				OrderDate = DateTime.UtcNow,
				UserId = 123,
				Total = 1,
				OrderGames =
				[
					new OrderItemRegisterDto { GameId = firstGameId },
					new OrderItemRegisterDto { GameId = secondGameId }
				]
			};

			_repositoryMock.Setup(r => r.GetById(orderId)).ReturnsAsync(existingOrder);
			_gameRepositoryMock.Setup(r => r.GetById(firstGameId)).ReturnsAsync(BuildGame(firstGameId, "Game 1", 100.00M, true));
			_gameRepositoryMock.Setup(r => r.GetById(secondGameId)).ReturnsAsync(BuildGame(secondGameId, "Game 2", 50.00M, true));

			var response = await _sut.Update(orderId, updateDto);

			Assert.True(response.IsSuccess);
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
			Assert.Equal(150.00M, existingOrder.Total);
			Assert.Equal(2, existingOrder.Items.Count);
			_repositoryMock.Verify(r => r.Update(orderId, existingOrder), Times.Once);
          _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task Update_ShouldReturnNotFound_WhenAnyGameDoesNotExist()
		{
			var orderId = Guid.NewGuid();
			var missingGameId = Guid.NewGuid();
			var existingOrder = BuildOrderWithSingleItem();
			var updateDto = BuildValidOrderUpdateDto(missingGameId);

			_repositoryMock.Setup(r => r.GetById(orderId)).ReturnsAsync(existingOrder);
			_gameRepositoryMock.Setup(r => r.GetById(missingGameId)).ReturnsAsync((Game?)null);

			var response = await _sut.Update(orderId, updateDto);

			Assert.False(response.IsSuccess);
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
			Assert.Equal($"Game not found: {missingGameId}", response.Message);
			_repositoryMock.Verify(r => r.Update(It.IsAny<Guid>(), It.IsAny<OrderAggregate>()), Times.Never);
		}

		[Fact]
		public async Task Update_ShouldReturnBadRequest_WhenAnyGameIsNotAvailable()
		{
			var orderId = Guid.NewGuid();
			var unavailableGameId = Guid.NewGuid();
			var existingOrder = BuildOrderWithSingleItem();
			var updateDto = BuildValidOrderUpdateDto(unavailableGameId);
			var unavailableGame = BuildGame(unavailableGameId, "Unavailable game", 25.00M, false);

			_repositoryMock.Setup(r => r.GetById(orderId)).ReturnsAsync(existingOrder);
			_gameRepositoryMock.Setup(r => r.GetById(unavailableGameId)).ReturnsAsync(unavailableGame);

			var response = await _sut.Update(orderId, updateDto);

			Assert.False(response.IsSuccess);
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			Assert.Equal("Game is not available: Unavailable game", response.Message);
			_repositoryMock.Verify(r => r.Update(It.IsAny<Guid>(), It.IsAny<OrderAggregate>()), Times.Never);
		}

		[Fact]
		public async Task UpdatePaymentStatus_ShouldReturnNotFound_WhenOrderDoesNotExist()
		{
			var orderId = Guid.NewGuid();
			_repositoryMock.Setup(r => r.GetById(orderId)).ReturnsAsync((OrderAggregate?)null);

			var response = await _sut.UpdatePaymentStatus(orderId, PaymentResultStatus.Approved);

			Assert.False(response.IsSuccess);
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
			Assert.Equal("Order not found.", response.Message);
			_repositoryMock.Verify(r => r.Update(It.IsAny<Guid>(), It.IsAny<OrderAggregate>()), Times.Never);
		}

		[Fact]
		public async Task UpdatePaymentStatus_ShouldMarkOrderAsPaid_WhenPaymentIsApproved()
		{
			var orderId = Guid.NewGuid();
			var order = BuildOrderWithSingleItem();
			order.RejectOrder();
			_repositoryMock.Setup(r => r.GetById(orderId)).ReturnsAsync(order);

			var response = await _sut.UpdatePaymentStatus(orderId, PaymentResultStatus.Approved);

			Assert.True(response.IsSuccess);
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
			Assert.Equal(OrderStatus.Paid, order.Status);
			_repositoryMock.Verify(r => r.Update(orderId, order), Times.Once);
          _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task UpdatePaymentStatus_ShouldMarkOrderAsRejected_WhenPaymentIsNotApproved()
		{
			var orderId = Guid.NewGuid();
			var order = BuildOrderWithSingleItem();
			_repositoryMock.Setup(r => r.GetById(orderId)).ReturnsAsync(order);

			var response = await _sut.UpdatePaymentStatus(orderId, PaymentResultStatus.Denied);

			Assert.True(response.IsSuccess);
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
			Assert.Equal(OrderStatus.Rejected, order.Status);
			_repositoryMock.Verify(r => r.Update(orderId, order), Times.Once);
          _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		private static OrderRegisterDto BuildValidOrderRegisterDto(Guid gameId)
			=> new()
			{
				OrderDate = DateTime.UtcNow,
				UserId = 123,
				OrderGames = [new OrderItemRegisterDto { GameId = gameId }]
			};

		private static OrderUpdateDto BuildValidOrderUpdateDto(Guid gameId)
			=> new()
			{
				OrderDate = DateTime.UtcNow,
				UserId = 123,
				Total = 10,
				OrderGames = [new OrderItemRegisterDto { GameId = gameId }]
			};

		private static OrderAggregate BuildOrderWithSingleItem()
		{
			var snapshot = new OrderItemSnapshot(Guid.NewGuid(), "Game", "Platform", "Publisher", "Description", 80.00M);
			return OrderAggregate.Create(DateTime.UtcNow, 123, [snapshot]);
		}

		private static Game BuildGame(Guid id, string name, decimal price, bool isAvailable)
		{
			var game = Game.Create(name, "Platform", "Publisher", "Description", price);
			return Game.Rehydrate(id, game.Name, game.Platform, game.PublisherName, game.Description, game.Price, isAvailable, DateTime.UtcNow);
   }
	}
}
