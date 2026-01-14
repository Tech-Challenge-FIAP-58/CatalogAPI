using System.ComponentModel.DataAnnotations;
using FIAP.FCG.CATALOG.Infra.Repository;
using FIAP.FCG.CATALOG.Core.Validation;
using FIAP.FCG.CATALOG.Core.Inputs;
using FIAP.FCG.CATALOG.Core.Web;

namespace FIAP.FCG.CATALOG.Application.Services
{
	public class OrderService(IOrderRepository repository) : BaseService, IOrderService
	{
        private readonly IOrderRepository _repository = repository;

        public async Task<int> Create(OrderRegisterDto orderRegisterDto)
        {
            return await _repository.Create(orderRegisterDto);
        }

        /*public async Task<IApiResponse<int>> Create(OrderRegisterDto orderRegisterDto)
        {
            var id = await _repository.Create(orderRegisterDto);
            return Created(id, "Pedido registrado com sucesso.");
        }*/

    }
}
