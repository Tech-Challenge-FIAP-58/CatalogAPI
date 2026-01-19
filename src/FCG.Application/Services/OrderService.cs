using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Web;
using FCG.Infra.Repository;

namespace FCG.Application.Services
{
    public class OrderService(IOrderRepository repository) : BaseService, IOrderService
    {
        private readonly IOrderRepository _repository = repository;

        public async Task<int> Create(OrderRegisterDto orderRegisterDto)
        {
            return await _repository.Create(orderRegisterDto);
        }

        public async Task<OrderResponseDto?> GetById(int id)
        {
            return await _repository.GetById(id);
        }

        public async Task<IApiResponse<bool>> Update(int id, OrderUpdateDto updateDto)
        {
            var ok = await _repository.Update(id, updateDto);
            return ok
                ? NoContent()
                : NotFound<bool>("Usuário não encontrado para atualização.");
        }

        /*public async Task<IApiResponse<int>> Create(OrderRegisterDto orderRegisterDto)
        {
            var id = await _repository.Create(orderRegisterDto);
            return Created(id, "Pedido registrado com sucesso.");
        }*/

    }
}
