using FCG.Catalog.Application.Interfaces;
using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Web;
using FCG.Catalog.Infra.Repository;
using FCG.Core.Integration;

namespace FCG.Catalog.Application.Services
{
    public class OrderService(IOrderRepository repository, IOrderPlacedEventProducer orderPlacedEventProducer) : BaseService, IOrderService
    {
        public async Task<IApiResponse<Guid>> Create(OrderRegisterDto orderRegisterDto)
        {
            var orderId = await repository.Create(orderRegisterDto);
			var orderRegistered = MapToPayment(orderId, orderRegisterDto);

			await orderPlacedEventProducer.Send(orderRegistered);

			return Success(orderId, message: $"Ordem #{orderId} criada com sucesso");
		}

        public async Task<OrderResponseDto?> GetById(Guid id)
        {
            return await repository.GetById(id);
        }

        public async Task<IApiResponse<bool>> Update(Guid id, OrderUpdateDto updateDto)
        {
            var ok = await repository.Update(id, updateDto);
            return ok
                ? NoContent()
                : NotFound<bool>("Usuário não encontrado para atualização.");
        }

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
