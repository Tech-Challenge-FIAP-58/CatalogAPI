using AutoMapper;
using FCG.Catalog.Application.Interfaces;
using FCG.Catalog.Application.Services;
using FCG.Catalog.Domain.Enums;
using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Models.Cart;
using FCG.Catalog.Domain.Models.Catalog;
using FCG.Catalog.Domain.Web;
using FCG.Catalog.Infra.Repository;
using Moq;
using System.Net;

namespace FCG.Catalog.Tests;

public class CartTests
{
    private readonly Mock<ICartRepository> _repositoryMock;
    private readonly Mock<IGameRepository> _gameRepositoryMock;
    private readonly Mock<IOrderService> _orderServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CartService _sut;

    public CartTests()
    {
        _repositoryMock = new Mock<ICartRepository>();
        _gameRepositoryMock = new Mock<IGameRepository>();
        _orderServiceMock = new Mock<IOrderService>();
        _mapperMock = new Mock<IMapper>();

        _mapperMock
            .Setup(m => m.Map<CartResponseDto>(It.IsAny<Cart>()))
            .Returns((Cart cart) => ToResponse(cart));

        _sut = new CartService(_repositoryMock.Object, _gameRepositoryMock.Object, _orderServiceMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetByUserId_ShouldCreateCart_WhenCartDoesNotExist()
    {
        const int userId = 42;
        _repositoryMock.Setup(r => r.GetByUserId(userId)).ReturnsAsync((Cart?)null);

        var response = await _sut.GetByUserId(userId);

        Assert.True(response.IsSuccess);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.ResultValue);
        Assert.Equal(userId, response.ResultValue!.UserId);
        Assert.Equal(CartStatus.Active, response.ResultValue.Status);
        _repositoryMock.Verify(r => r.Create(It.IsAny<Cart>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByUserId_ShouldReturnExistingCart_WhenCartAlreadyExists()
    {
        const int userId = 42;
        var existingCart = Cart.Create(userId);
        existingCart.AddItem(Guid.NewGuid(), "GTA", "PC", "Rockstar", "Desc", 200);
        _repositoryMock.Setup(r => r.GetByUserId(userId)).ReturnsAsync(existingCart);

        var response = await _sut.GetByUserId(userId);

        Assert.True(response.IsSuccess);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.ResultValue);
        Assert.Equal(userId, response.ResultValue!.UserId);
        Assert.Single(response.ResultValue.Items);
        _repositoryMock.Verify(r => r.Create(It.IsAny<Cart>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AddItem_ShouldReturnNotFound_WhenGameDoesNotExist()
    {
        var dto = new CartAddItemDto { UserId = 1, GameId = Guid.NewGuid() };
        _gameRepositoryMock.Setup(r => r.GetById(dto.GameId)).ReturnsAsync((Game?)null);

        var response = await _sut.AddItem(dto);

        Assert.False(response.IsSuccess);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("Game not found.", response.Message);
        _repositoryMock.Verify(r => r.Create(It.IsAny<Cart>()), Times.Never);
        _repositoryMock.Verify(r => r.Update(It.IsAny<Cart>()), Times.Never);
    }

    [Fact]
    public async Task AddItem_ShouldReturnBadRequest_WhenGameIsNotAvailable()
    {
        var dto = new CartAddItemDto { UserId = 1, GameId = Guid.NewGuid() };
        var game = BuildGame(dto.GameId, "GTA", 200, false);
        _gameRepositoryMock.Setup(r => r.GetById(dto.GameId)).ReturnsAsync(game);

        var response = await _sut.AddItem(dto);

        Assert.False(response.IsSuccess);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Game is not available.", response.Message);
        _repositoryMock.Verify(r => r.Create(It.IsAny<Cart>()), Times.Never);
    }

    [Fact]
    public async Task AddItem_ShouldCreateCartAndAddItem_WhenCartDoesNotExist()
    {
        var dto = new CartAddItemDto { UserId = 1, GameId = Guid.NewGuid() };
        var game = BuildGame(dto.GameId, "GTA", 200, true);

        _gameRepositoryMock.Setup(r => r.GetById(dto.GameId)).ReturnsAsync(game);
        _repositoryMock.Setup(r => r.GetByUserId(dto.UserId)).ReturnsAsync((Cart?)null);

        var response = await _sut.AddItem(dto);

        Assert.True(response.IsSuccess);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.ResultValue);
        Assert.Single(response.ResultValue!.Items);
        Assert.Equal(200, response.ResultValue.Total);
        _repositoryMock.Verify(r => r.Create(It.IsAny<Cart>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddItem_ShouldReturnBadRequest_WhenGameIsAlreadyInCart()
    {
        var dto = new CartAddItemDto { UserId = 1, GameId = Guid.NewGuid() };
        var existingCart = Cart.Create(dto.UserId);
        existingCart.AddItem(dto.GameId, "GTA", "PC", "Rockstar", "Desc", 200);
        var game = BuildGame(dto.GameId, "GTA", 200, true);

        _gameRepositoryMock.Setup(r => r.GetById(dto.GameId)).ReturnsAsync(game);
        _repositoryMock.Setup(r => r.GetByUserId(dto.UserId)).ReturnsAsync(existingCart);

        var response = await _sut.AddItem(dto);

        Assert.False(response.IsSuccess);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Game is already added to cart. (Parameter 'gameId')", response.Message);
        _repositoryMock.Verify(r => r.Update(It.IsAny<Cart>()), Times.Never);
    }

    [Fact]
    public async Task AddItem_ShouldUpdateCart_WhenCartAlreadyExistsAndGameIsNew()
    {
        var dto = new CartAddItemDto { UserId = 1, GameId = Guid.NewGuid() };
        var existingCart = Cart.Create(dto.UserId);
        existingCart.AddItem(Guid.NewGuid(), "RDR2", "PC", "Rockstar", "Desc", 150);
        var game = BuildGame(dto.GameId, "GTA", 200, true);

        _gameRepositoryMock.Setup(r => r.GetById(dto.GameId)).ReturnsAsync(game);
        _repositoryMock.Setup(r => r.GetByUserId(dto.UserId)).ReturnsAsync(existingCart);

        var response = await _sut.AddItem(dto);

        Assert.True(response.IsSuccess);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.ResultValue);
        Assert.Equal(2, response.ResultValue!.Items.Count);
        Assert.Equal(350, response.ResultValue.Total);
        _repositoryMock.Verify(r => r.Update(existingCart), Times.Once);
        _repositoryMock.Verify(r => r.Create(It.IsAny<Cart>()), Times.Never);
    }

    [Fact]
    public async Task RemoveItem_ShouldReturnNotFound_WhenCartDoesNotExist()
    {
        var dto = new CartRemoveItemDto { UserId = 1, GameId = Guid.NewGuid() };
        _repositoryMock.Setup(r => r.GetByUserId(dto.UserId)).ReturnsAsync((Cart?)null);

        var response = await _sut.RemoveItem(dto);

        Assert.False(response.IsSuccess);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("Cart not found.", response.Message);
    }

    [Fact]
    public async Task Checkout_ShouldReturnBadRequest_WhenCartIsEmpty()
    {
        const int userId = 10;
        var cart = Cart.Create(userId);
        var checkoutDto = BuildValidCheckoutDto(userId, 100);

        _repositoryMock.Setup(r => r.GetByUserId(userId)).ReturnsAsync(cart);

        var response = await _sut.Checkout(checkoutDto);

        Assert.False(response.IsSuccess);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Cart is empty.", response.Message);
        _orderServiceMock.Verify(s => s.Create(It.IsAny<OrderRegisterDto>(), It.IsAny<CheckoutCartDto>()), Times.Never);
    }

    [Fact]
    public async Task Checkout_ShouldReturnNotFound_WhenCartContainsGameThatDoesNotExist()
    {
        const int userId = 10;
        var gameId = Guid.NewGuid();
        var cart = Cart.Create(userId);
        cart.AddItem(gameId, "GTA", "PC", "Rockstar", "Desc", 100);
        var checkoutDto = BuildValidCheckoutDto(userId, 100);

        _repositoryMock.Setup(r => r.GetByUserId(userId)).ReturnsAsync(cart);
        _gameRepositoryMock.Setup(r => r.GetById(gameId)).ReturnsAsync((Game?)null);

        var response = await _sut.Checkout(checkoutDto);

        Assert.False(response.IsSuccess);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal($"Game not found: {gameId}", response.Message);
        _orderServiceMock.Verify(s => s.Create(It.IsAny<OrderRegisterDto>(), It.IsAny<CheckoutCartDto>()), Times.Never);
    }

    [Fact]
    public async Task Checkout_ShouldReturnBadRequest_WhenCartContainsUnavailableGame()
    {
        const int userId = 10;
        var gameId = Guid.NewGuid();
        var cart = Cart.Create(userId);
        cart.AddItem(gameId, "GTA", "PC", "Rockstar", "Desc", 100);
        var checkoutDto = BuildValidCheckoutDto(userId, 100);
        var unavailableGame = BuildGame(gameId, "GTA", 100, false);

        _repositoryMock.Setup(r => r.GetByUserId(userId)).ReturnsAsync(cart);
        _gameRepositoryMock.Setup(r => r.GetById(gameId)).ReturnsAsync(unavailableGame);

        var response = await _sut.Checkout(checkoutDto);

        Assert.False(response.IsSuccess);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Game is not available: GTA", response.Message);
        _orderServiceMock.Verify(s => s.Create(It.IsAny<OrderRegisterDto>(), It.IsAny<CheckoutCartDto>()), Times.Never);
    }

    [Fact]
    public async Task RemoveItem_ShouldRemoveItemAndPersist_WhenCartExists()
    {
        var dto = new CartRemoveItemDto { UserId = 1, GameId = Guid.NewGuid() };
        var cart = Cart.Create(dto.UserId);
        cart.AddItem(dto.GameId, "GTA", "PC", "Rockstar", "Desc", 200);
        _repositoryMock.Setup(r => r.GetByUserId(dto.UserId)).ReturnsAsync(cart);

        var response = await _sut.RemoveItem(dto);

        Assert.True(response.IsSuccess);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.ResultValue);
        Assert.Empty(response.ResultValue!.Items);
        Assert.Equal(0, response.ResultValue.Total);
        _repositoryMock.Verify(r => r.Update(cart), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveItem_ShouldKeepCartUnchanged_WhenGameIsNotInCart()
    {
        var dto = new CartRemoveItemDto { UserId = 1, GameId = Guid.NewGuid() };
        var otherGameId = Guid.NewGuid();
        var cart = Cart.Create(dto.UserId);
        cart.AddItem(otherGameId, "GTA", "PC", "Rockstar", "Desc", 200);
        _repositoryMock.Setup(r => r.GetByUserId(dto.UserId)).ReturnsAsync(cart);

        var response = await _sut.RemoveItem(dto);

        Assert.True(response.IsSuccess);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.ResultValue);
        Assert.Single(response.ResultValue!.Items);
        Assert.Equal(200, response.ResultValue.Total);
        _repositoryMock.Verify(r => r.Update(cart), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Clear_ShouldReturnNotFound_WhenCartDoesNotExist()
    {
        const int userId = 99;
        _repositoryMock.Setup(r => r.GetByUserId(userId)).ReturnsAsync((Cart?)null);

        var response = await _sut.Clear(userId);

        Assert.False(response.IsSuccess);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("Cart not found.", response.Message);
    }

    [Fact]
    public async Task Checkout_ShouldReturnBadRequest_WhenCheckoutDtoIsInvalid()
    {
        var invalidDto = new CheckoutCartDto
        {
            ClientId = 10,
            UserEmail = "test@gmail.com",
            PaymentMethod = PaymentMethod.CreditCard,
            Amount = 0,
            CardName = "A",
            CardNumber = "123",
            ExpirationDate = "1",
            Cvv = "1"
        };

        var response = await _sut.Checkout(invalidDto);

        Assert.False(response.IsSuccess);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.StartsWith("Invalid checkout data:", response.Message);
        _repositoryMock.Verify(r => r.GetByUserId(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Clear_ShouldClearCartAndPersist_WhenCartExists()
    {
        const int userId = 1;
        var cart = Cart.Create(userId);
        cart.AddItem(Guid.NewGuid(), "GTA", "PC", "Rockstar", "Desc", 200);
        _repositoryMock.Setup(r => r.GetByUserId(userId)).ReturnsAsync(cart);

        var response = await _sut.Clear(userId);

        Assert.True(response.IsSuccess);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.ResultValue);
        Assert.Empty(response.ResultValue!.Items);
        Assert.Equal(0, response.ResultValue.Total);
        _repositoryMock.Verify(r => r.Update(cart), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Checkout_ShouldReturnNotFound_WhenCartDoesNotExist()
    {
        var checkoutDto = BuildValidCheckoutDto(10, 100);
        _repositoryMock.Setup(r => r.GetByUserId(checkoutDto.ClientId)).ReturnsAsync((Cart?)null);

        var response = await _sut.Checkout(checkoutDto);

        Assert.False(response.IsSuccess);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("Cart not found.", response.Message);
    }

    [Fact]
    public async Task Checkout_ShouldReturnBadRequest_WhenCartTotalDoesNotMatchRecalculatedTotal()
    {
        const int userId = 10;
        var gameId = Guid.NewGuid();
        var cart = Cart.Create(userId);
        cart.AddItem(gameId, "GTA", "PC", "Rockstar", "Desc", 100);
        var checkoutDto = BuildValidCheckoutDto(userId, 100);
        var changedPriceGame = BuildGame(gameId, "GTA", 150, true);

        _repositoryMock.Setup(r => r.GetByUserId(userId)).ReturnsAsync(cart);
        _gameRepositoryMock.Setup(r => r.GetById(gameId)).ReturnsAsync(changedPriceGame);

        var response = await _sut.Checkout(checkoutDto);

        Assert.False(response.IsSuccess);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Cart total does not match recalculated total.", response.Message);
        _orderServiceMock.Verify(s => s.Create(It.IsAny<OrderRegisterDto>(), It.IsAny<CheckoutCartDto>()), Times.Never);
    }

    [Fact]
    public async Task Checkout_ShouldReturnBadRequest_WhenPaymentAmountIsInvalid()
    {
        const int userId = 10;
        var gameId = Guid.NewGuid();
        var cart = Cart.Create(userId);
        cart.AddItem(gameId, "GTA", "PC", "Rockstar", "Desc", 100);
        var checkoutDto = BuildValidCheckoutDto(userId, 90);
        var game = BuildGame(gameId, "GTA", 100, true);

        _repositoryMock.Setup(r => r.GetByUserId(userId)).ReturnsAsync(cart);
        _gameRepositoryMock.Setup(r => r.GetById(gameId)).ReturnsAsync(game);

        var response = await _sut.Checkout(checkoutDto);

        Assert.False(response.IsSuccess);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Invalid payment amount.", response.Message);
    }

    [Fact]
    public async Task Checkout_ShouldPropagateOrderServiceFailure_WhenOrderCreationFails()
    {
        const int userId = 10;
        var gameId = Guid.NewGuid();
        var cart = Cart.Create(userId);
        cart.AddItem(gameId, "GTA", "PC", "Rockstar", "Desc", 100);
        var checkoutDto = BuildValidCheckoutDto(userId, 100);
        var game = BuildGame(gameId, "GTA", 100, true);
        var orderFailure = new TestApiResponse<Guid?>
        {
            IsSuccess = false,
            StatusCode = HttpStatusCode.BadRequest,
            Message = "Order creation failed.",
            ResultValue = null
        };

        _repositoryMock.Setup(r => r.GetByUserId(userId)).ReturnsAsync(cart);
        _gameRepositoryMock.Setup(r => r.GetById(gameId)).ReturnsAsync(game);
        _orderServiceMock.Setup(s => s.Create(It.IsAny<OrderRegisterDto>(), checkoutDto)).ReturnsAsync(orderFailure);

        var response = await _sut.Checkout(checkoutDto);

        Assert.False(response.IsSuccess);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Order creation failed.", response.Message);
        Assert.Equal(CartStatus.Active, cart.Status);
        _repositoryMock.Verify(r => r.Update(It.IsAny<Cart>()), Times.Never);
    }

    [Fact]
    public async Task Checkout_ShouldPropagateOrderResponse_WhenOrderIdIsNull()
    {
        const int userId = 10;
        var gameId = Guid.NewGuid();
        var cart = Cart.Create(userId);
        cart.AddItem(gameId, "GTA", "PC", "Rockstar", "Desc", 100);
        var checkoutDto = BuildValidCheckoutDto(userId, 100);
        var game = BuildGame(gameId, "GTA", 100, true);
        var orderResponseWithNullId = new TestApiResponse<Guid?>
        {
            IsSuccess = true,
            StatusCode = HttpStatusCode.Created,
            Message = "Order created but id missing.",
            ResultValue = null
        };

        _repositoryMock.Setup(r => r.GetByUserId(userId)).ReturnsAsync(cart);
        _gameRepositoryMock.Setup(r => r.GetById(gameId)).ReturnsAsync(game);
        _orderServiceMock.Setup(s => s.Create(It.IsAny<OrderRegisterDto>(), checkoutDto)).ReturnsAsync(orderResponseWithNullId);

        var response = await _sut.Checkout(checkoutDto);

        Assert.True(response.IsSuccess);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("Order created but id missing.", response.Message);
        Assert.Null(response.ResultValue);
        Assert.Equal(CartStatus.Active, cart.Status);
        _repositoryMock.Verify(r => r.Update(It.IsAny<Cart>()), Times.Never);
    }

    [Fact]
    public async Task Checkout_ShouldCompleteCartAndReturnCreated_WhenOrderCreationSucceeds()
    {
        const int userId = 10;
        var gameId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var cart = Cart.Create(userId);
        cart.AddItem(gameId, "GTA", "PC", "Rockstar", "Desc", 100);
        var checkoutDto = BuildValidCheckoutDto(userId, 100);
        var game = BuildGame(gameId, "GTA", 100, true);
        var orderSuccess = new TestApiResponse<Guid?>
        {
            IsSuccess = true,
            StatusCode = HttpStatusCode.Created,
            Message = "Order created successfully.",
            ResultValue = orderId
        };

        _repositoryMock.Setup(r => r.GetByUserId(userId)).ReturnsAsync(cart);
        _gameRepositoryMock.Setup(r => r.GetById(gameId)).ReturnsAsync(game);
        _orderServiceMock
            .Setup(s => s.Create(
                It.Is<OrderRegisterDto>(o =>
                    o.UserId == userId &&
                    o.OrderGames.Count == 1 &&
                    o.OrderGames[0].GameId == gameId),
                checkoutDto))
            .ReturnsAsync(orderSuccess);

        var response = await _sut.Checkout(checkoutDto);

        Assert.True(response.IsSuccess);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("Order created.", response.Message);
        Assert.Equal(orderId, response.ResultValue);
        Assert.Equal(CartStatus.Completed, cart.Status);
        _repositoryMock.Verify(r => r.Update(cart), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static CheckoutCartDto BuildValidCheckoutDto(int userId, decimal amount)
        => new()
        {
            ClientId = userId,
            UserEmail = "test@gmail.com",
            PaymentMethod = PaymentMethod.CreditCard,
            Amount = amount,
            CardName = "John Doe",
            CardNumber = "123456789012",
            ExpirationDate = "12/30",
            Cvv = "123"
        };

    private static Game BuildGame(Guid id, string name, decimal price, bool isAvailable)
    {
        var game = Game.Create(name, "PC", "Rockstar", "Desc", price);
        return Game.Rehydrate(id, game.Name, game.Platform, game.PublisherName, game.Description, game.Price, isAvailable, DateTime.UtcNow);
    }

    private static CartResponseDto ToResponse(Cart cart)
        => new(
            cart.Id,
            cart.UserId,
            cart.Total,
            cart.Items.Select(i => new CartItemResponseDto(
                i.GameId,
                i.Name,
                i.Platform,
                i.PublisherName,
                i.Description,
                i.UnitPrice,
                i.Quantity,
                i.Total)).ToList(),
            cart.Status);

    private sealed class TestApiResponse<T> : IApiResponse<T>
    {
        public T? ResultValue { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
    }
}
