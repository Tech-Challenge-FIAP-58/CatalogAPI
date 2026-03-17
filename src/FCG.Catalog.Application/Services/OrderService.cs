using AutoMapper;
using FCG.Catalog.Application.Interfaces;
using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Validation;
using FCG.Catalog.Domain.Web;
using FCG.Catalog.Infra.Repository;
using FCG.Core.Integration;
using System.ComponentModel.DataAnnotations;

namespace FCG.Catalog.Application.Services
{
    public class OrderService(IOrderRepository _repository, IMapper _mapper) : BaseService, IOrderService
    {
        public async Task<IApiResponse<Guid?>> Create(OrderRegisterDto orderRegisterDto)
        {
            try
            {
                DtoValidator.ValidateObject(orderRegisterDto);
            }
            catch (ValidationException ex)
            {
                return BadRequest<Guid?>($"Dados de jogo inválidos: {ex.Message}");
            }

            var order = OrderRegisterDto.ToOrder(orderRegisterDto);
            order.AuthorizeOrder();

            var id = await _repository.Create(order);

            return Created<Guid?>(id, "Jogo registrado com sucesso.");
        }

        public async Task<IApiResponse<OrderResponseDto?>> GetById(Guid id)
        {
            var order = await _repository.GetById(id);

            return order is null
                ? NotFound<OrderResponseDto?>("Jogo não encontrado.")
                : Ok<OrderResponseDto?>(_mapper.Map<OrderResponseDto>(order));
        }

        public async Task<IApiResponse<bool>> Update(Guid id, OrderUpdateDto updateDto)
        {
            var order = await _repository.GetById(id);

            if (order is null)
                return NotFound<bool>("Order not found.");

            order.Update(updateDto.OrderDate, updateDto.UserId, updateDto.Price, updateDto.PaymentStatus);

            return order
                ? NoContent()
                : NotFound<bool>("Usuário não encontrado para atualização.");
        }

        //public async Task<IApiResponse<bool>> Update(Guid id, GameUpdateDto updateDto)
        //{
        //    var game = await _repository.GetById(id);

        //    if (game is null)
        //        return NotFound<bool>("Jogo não encontrado para atualização.");

        //    game.Update(updateDto.Name, updateDto.Platform, updateDto.PublisherName, updateDto.Description, updateDto.Price);

        //    await _repository.Update(game);

        //    return NoContent();
        //}


        #region private ::

        private static OrderPlacedEvent MapToPayment(Guid orderId, OrderRegisterDto message)
		{
			return new OrderPlacedEvent(
					message.UserId,
					orderId,
					1,
					message.Price,
					message.CardName,
					message.CardNumber,
					message.ExpirationDate,
					message.Cvv
				);
		}

        #endregion
    }
}
