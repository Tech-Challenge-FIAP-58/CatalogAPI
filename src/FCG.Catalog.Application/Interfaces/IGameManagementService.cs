using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Web;

namespace FCG.Catalog.Application.Interfaces
{
    public interface IGameManagementService
    {
        Task<IApiResponse<Guid?>> Create(GameRegisterDto register);
        Task<IApiResponse<bool>> Update(Guid id, GameUpdateDto update);
        Task<IApiResponse<bool>> Remove(Guid id);
    }
}
