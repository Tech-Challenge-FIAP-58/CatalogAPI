

using FIAP.FCG.CATALOG.Core.Inputs;
using FIAP.FCG.CATALOG.Core.Models;
using FIAP.FCG.CATALOG.Core.Web;
using System.Threading.Tasks;

namespace FIAP.FCG.CATALOG.Application.Services
{
    public interface IOrderService
    {
        Task<int> Create(OrderRegisterDto orderRegisterDto);
        Task<OrderResponseDto?> GetById(int id);
        Task<IApiResponse<bool>> Update(int id, OrderUpdateDto update);


    }
}
