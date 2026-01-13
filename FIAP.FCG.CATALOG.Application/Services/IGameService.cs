using FIAP.FCG.CATALOG.Core.Inputs;
using FIAP.FCG.CATALOG.Core.Models;
using FIAP.FCG.CATALOG.Core.Web;

namespace FIAP.FCG.CATALOG.Application.Services
{
	public interface IOrderService
	{
		Task<IApiResponse<int>> Create(OrderRegisterDto register);
    }
}
