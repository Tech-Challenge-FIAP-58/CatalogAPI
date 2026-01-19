using FCG.Catalog.Application.Interfaces;
using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Web;
using FCG.Catalog.Infra.Repository;

namespace FCG.Catalog.Application.Services
{
    public class CatalogService(ICatalogRepository repository) : BaseService, ICatalogService
    {
        public async Task<IApiResponse<IEnumerable<CatalogResponseDto>>> GetAll()
        {
            var list = await repository.GetAll();
            return Ok(list);
        }

        public async Task<IApiResponse<CatalogResponseDto?>> GetByUserId(int id)
        {
            var dto = await repository.GetByUserId(id);
            return dto is null
                ? NotFound<CatalogResponseDto?>("Catálogo não encontrado para este usuário.")
                : Ok<CatalogResponseDto?>(dto);
        }

        public async Task<IApiResponse<CatalogRegisterDto?>> Create(CatalogRegisterDto dto)
        {
            /*CatalogRegisterDto dto = new CatalogRegisterDto();
            dto.UserId = UserId;
            dto.GameId = GameId;
            dto.Price = price;*/

            await repository.Create(dto);
            return dto is null
                ? NotFound<CatalogRegisterDto?>("Catálogo não encontrado para este usuário.")
                : Ok<CatalogRegisterDto?>(dto);
        }
    }
}
