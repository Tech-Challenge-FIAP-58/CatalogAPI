using AutoMapper;
using FCG.Catalog.Application.Interfaces;
using FCG.Catalog.Domain.Events;
using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Validation;
using FCG.Core.Integration;
using OrderAggregate = FCG.Catalog.Domain.Models.Order.Order;
using FCG.Catalog.Domain.Web;
using FCG.Catalog.Infra.Repository;
using System.ComponentModel.DataAnnotations;

namespace FCG.Catalog.Application.Services
{
    public class OrderService(IOrderRepository _repository, IGameRepository _gameRepository, IOrderPlacedEventProducer _orderPlacedEventProducer, IMapper _mapper) : BaseService, IOrderService
    {
        public async Task<IApiResponse<Guid?>> Create(OrderRegisterDto orderRegisterDto, CheckoutCartDto? checkoutDto = null)
        {
            try
            {
                DtoValidator.ValidateObject(orderRegisterDto);
            }
            catch (ValidationException ex)
            {
                return BadRequest<Guid?>($"Invalid order data: {ex.Message}");
            }

            var gameIds = orderRegisterDto.OrderGames
                .Select(orderGame => orderGame.GameId)
                .ToList();

            if (gameIds.GroupBy(gameId => gameId).Any(group => group.Count() > 1))
            {
                return BadRequest<Guid?>("Order already contains duplicated games.");
            }

            var gameSnapshots = new List<(Guid Id, string Name, string Platform, string PublisherName, string Description, decimal Price)>();

            foreach (var gameId in gameIds)
            {
                var game = await _gameRepository.GetById(gameId);

                if (game is null)
                {
                    return NotFound<Guid?>($"Game not found: {gameId}");
                }

                if (!game.IsAvailable)
                {
                    return BadRequest<Guid?>($"Game is not available: {game.Name}");
                }

                gameSnapshots.Add((
                    game.Id,
                    game.Name,
                    game.Platform,
                    game.PublisherName,
                    game.Description,
                    game.Price));
            }

            var orderItems = gameSnapshots
                .Select(game => new OrderItemSnapshot(
                    game.Id,
                    game.Name,
                    game.Platform,
                    game.PublisherName,
                    game.Description,
                    game.Price))
                .ToList();

            OrderAggregate order;

            try
            {
                order = OrderAggregate.Create(orderRegisterDto.OrderDate, orderRegisterDto.UserId, orderItems);
            }
            catch (ArgumentException ex)
            {
                return BadRequest<Guid?>(ex.Message);
            }

            var id = _repository.Create(order);

            await _repository.SaveChangesAsync();

            if (checkoutDto is not null)
            {
                await _orderPlacedEventProducer.Send(new OrderPlacedEvent(
                    checkoutDto.ClientId,
                    id,
                    (FCG.Core.Integration.PaymentMethod)checkoutDto.PaymentMethod,
                    checkoutDto.Amount,
                    checkoutDto.CardName,
                    checkoutDto.CardNumber,
                    checkoutDto.ExpirationDate,
                    checkoutDto.Cvv));
            }

            return Created<Guid?>(id, "Order created successfully.");
        }

        public async Task<IApiResponse<OrderResponseDto?>> GetById(Guid id)
        {
            var order = await _repository.GetById(id);

            return order is null
                ? NotFound<OrderResponseDto?>("Order not found.")
                : Ok<OrderResponseDto?>(_mapper.Map<OrderResponseDto>(order));
        }

        public async Task<IApiResponse<IReadOnlyCollection<OrderResponseDto>>> GetByUserId(int userId)
        {
            var orders = await _repository.GetByUserId(userId);

            var response = _mapper.Map<IReadOnlyCollection<OrderResponseDto>>(orders);

            return Ok(response);
        }

        public async Task<IApiResponse<bool>> UpdatePaymentStatus(Guid id, PaymentResultStatus paymentStatus)
        {
            var order = await _repository.GetById(id);

            if (order is null)
            {
                return NotFound<bool>("Order not found.");
            }

            if (paymentStatus == PaymentResultStatus.Approved)
            {
                order.PayOrder();
            }
            else
            {
                order.RejectOrder();
            }

            _repository.Update(id, order);

            await _repository.SaveChangesAsync();

            return NoContent();
        }

        public async Task<IApiResponse<bool>> Update(Guid id, OrderUpdateDto updateDto)
        {
            try
            {
                DtoValidator.ValidateObject(updateDto);
            }
            catch (ValidationException ex)
            {
                return BadRequest<bool>($"Invalid order data: {ex.Message}");
            }

            var order = await _repository.GetById(id);

            if (order is null)
                return NotFound<bool>("Order not found.");

            var gameIds = updateDto.OrderGames
                .Select(orderGame => orderGame.GameId)
                .ToList();

            if (gameIds.GroupBy(gameId => gameId).Any(group => group.Count() > 1))
            {
                return BadRequest<bool>("Order already contains duplicated games.");
            }

            var gameSnapshots = new List<(Guid Id, string Name, string Platform, string PublisherName, string Description, decimal Price)>();

            foreach (var gameId in gameIds)
            {
                var game = await _gameRepository.GetById(gameId);

                if (game is null)
                {
                    return NotFound<bool>($"Game not found: {gameId}");
                }

                if (!game.IsAvailable)
                {
                    return BadRequest<bool>($"Game is not available: {game.Name}");
                }

                gameSnapshots.Add((
                    game.Id,
                    game.Name,
                    game.Platform,
                    game.PublisherName,
                    game.Description,
                    game.Price));
            }

            var orderItems = gameSnapshots
                .Select(game => new OrderItemSnapshot(
                    game.Id,
                    game.Name,
                    game.Platform,
                    game.PublisherName,
                    game.Description,
                    game.Price))
                .ToList();

            var total = orderItems.Sum(item => item.Price);

            try
            {
                order.Update(updateDto.OrderDate, updateDto.UserId, total, orderItems);
            }
            catch (ArgumentException ex)
            {
                return BadRequest<bool>(ex.Message);
            }

            _repository.Update(id, order);

            await _repository.SaveChangesAsync();

            return true
                ? NoContent()
                : NotFound<bool>("User not found for update.");
        }
    }
}
