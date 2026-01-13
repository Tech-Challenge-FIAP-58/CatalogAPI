using FIAP.FCG.CATALOG.Core.Inputs;
using FIAP.FCG.CATALOG.Core.Models;

namespace FIAP.FCG.CATALOG.Infra.Repository
{
	public interface IOrderRepository
	{
		Task<int> Create(OrderRegisterDto orderRegister);

    }
}
