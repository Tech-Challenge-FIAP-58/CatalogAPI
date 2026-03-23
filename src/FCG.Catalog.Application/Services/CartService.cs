using System.ComponentModel.DataAnnotations;
using FCG.Catalog.Application.Interfaces;
using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Models.Cart;
using FCG.Catalog.Domain.Repository;
using FCG.Catalog.Domain.Validation;
using FCG.Catalog.Domain.Web;
using AutoMapper;

namespace FCG.Catalog.Application.Services
{
    public class CartService(
        ICartRepository repository,
        IGameCatalogLookupService gameCatalogLookupService,
        IOrderCheckoutService orderCheckoutService,
        IMapper mapper) : BaseService, ICartReadService, ICartManagementService
    {
        public async Task<IApiResponse<CartResponseDto?>> GetByUserId(int userId)
        {
            var cart = await repository.GetByUserId(userId);

            if (cart is null)
            {
                cart = Cart.Create(userId);
                repository.Create(cart);
                await repository.SaveChangesAsync();
            }

            return Ok<CartResponseDto?>(mapper.Map<CartResponseDto>(cart));
        }

        public async Task<IApiResponse<CartResponseDto?>> AddItem(CartAddItemDto dto)
        {
            try
            {
                DtoValidator.ValidateObject(dto);
            }
            catch (ValidationException ex)
            {
                return BadRequest<CartResponseDto?>($"Invalid cart data: {ex.Message}");
            }

            var game = await gameCatalogLookupService.GetByIdForProcessing(dto.GameId);

            if (game is null)
            {
                return NotFound<CartResponseDto?>("Game not found.");
            }

            if (!game.IsAvailable)
            {
                return BadRequest<CartResponseDto?>("Game is not available.");
            }

            var cart = await repository.GetByUserId(dto.UserId);
            var isNew = cart is null;

            if (isNew)
            {
                cart = Cart.Create(dto.UserId);
            }

            try
            {
                cart.AddItem(
                    game.Id,
                    game.Name,
                    game.Platform,
                    game.PublisherName,
                    game.Description,
                    game.Price);
            }
            catch (ArgumentException ex)
            {
                return BadRequest<CartResponseDto?>(ex.Message);
            }

            if (isNew)
            {
                repository.Create(cart);
            }
            else
            {
                repository.Update(cart);
            }

            await repository.SaveChangesAsync();

            return Ok<CartResponseDto?>(mapper.Map<CartResponseDto>(cart));
        }

        public async Task<IApiResponse<CartResponseDto?>> RemoveItem(CartRemoveItemDto dto)
        {
            try
            {
                DtoValidator.ValidateObject(dto);
            }
            catch (ValidationException ex)
            {
                return BadRequest<CartResponseDto?>($"Invalid cart data: {ex.Message}");
            }

            var cart = await repository.GetByUserId(dto.UserId);

            if (cart is null)
            {
                return NotFound<CartResponseDto?>("Cart not found.");
            }

            cart.RemoveItem(dto.GameId);
            repository.Update(cart);
            await repository.SaveChangesAsync();

            return Ok<CartResponseDto?>(mapper.Map<CartResponseDto>(cart));
        }

        public async Task<IApiResponse<CartResponseDto?>> Clear(int userId)
        {
            var cart = await repository.GetByUserId(userId);

            if (cart is null)
            {
                return NotFound<CartResponseDto?>("Cart not found.");
            }

            cart.Clear();
            repository.Update(cart);
            await repository.SaveChangesAsync();

            return Ok<CartResponseDto?>(mapper.Map<CartResponseDto>(cart));
        }

        public async Task<IApiResponse<Guid?>> Checkout(CheckoutCartDto dto)
        {
            try
            {
                DtoValidator.ValidateObject(dto);
            }
            catch (ValidationException ex)
            {
                return BadRequest<Guid?>($"Invalid checkout data: {ex.Message}");
            }

            var cart = await repository.GetByUserId(dto.ClientId);

            if (cart is null)
            {
                return NotFound<Guid?>("Cart not found.");
            }

            if (!cart.Items.Any())
            {
                return BadRequest<Guid?>("Cart is empty.");
            }

            var recalculatedCartItems = new List<(Guid GameId, string Name, decimal Price, int Quantity)>();

            foreach (var cartItem in cart.Items)
            {
                var game = await gameCatalogLookupService.GetByIdForProcessing(cartItem.GameId);

                if (game is null)
                {
                    return NotFound<Guid?>($"Game not found: {cartItem.GameId}");
                }

                if (!game.IsAvailable)
                {
                    return BadRequest<Guid?>($"Game is not available: {game.Name}");
                }

                recalculatedCartItems.Add((game.Id, game.Name, game.Price, cartItem.Quantity));
            }

            var recalculatedCartTotal = recalculatedCartItems.Sum(item => item.Price * item.Quantity);

            if (recalculatedCartTotal != cart.Total)
            {
                return BadRequest<Guid?>("Cart total does not match recalculated total.");
            }

            if (dto.Amount != recalculatedCartTotal)
            {
                return BadRequest<Guid?>("Invalid payment amount.");
            }

            var createOrderDto = new OrderRegisterDto
            {
                OrderDate = DateTime.UtcNow,
                UserId = dto.ClientId,
                OrderGames = recalculatedCartItems
                    .SelectMany(item => Enumerable.Range(0, item.Quantity)
                        .Select(_ => new OrderItemRegisterDto { GameId = item.GameId }))
                    .ToList()
            };

            var createOrderResponse = await orderCheckoutService.Create(createOrderDto, dto);

            if (!createOrderResponse.IsSuccess || createOrderResponse.ResultValue is null)
            {
                return createOrderResponse;
            }

            var orderId = createOrderResponse.ResultValue.Value;

            cart.Complete();
            repository.Update(cart);

            await repository.SaveChangesAsync();

            return Created<Guid?>(orderId, "Order created.");
        }
    }
}
