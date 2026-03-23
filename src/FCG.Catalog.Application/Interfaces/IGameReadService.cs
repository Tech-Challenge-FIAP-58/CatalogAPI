using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Web;

namespace FCG.Catalog.Application.Interfaces
{
    public interface IGameReadService
    {
        Task<IApiResponse<IEnumerable<GameResponseDto>>> GetAll();
        Task<IApiResponse<GameResponseDto?>> GetById(Guid id);
    }
}
