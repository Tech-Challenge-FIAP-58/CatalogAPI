using FIAP.FCG.CATALOG.Core.Inputs;
using FIAP.FCG.CATALOG.Core.Web;
using FIAP.FCG.CATALOG.Infra.Repository;

namespace FIAP.FCG.CATALOG.Application.Services
{
    public class CatalogService(ICatalogRepository repository) : BaseService, ICatalogService
    {
        private readonly ICatalogRepository _catalogRepository = repository;

        public async Task<IApiResponse<IEnumerable<CatalogResponseDto>>> GetAll()
        {
            var list = await _catalogRepository.GetAll();
            return Ok(list);
        }

        public async Task<IApiResponse<CatalogResponseDto?>> GetByUserId(int id)
        {
            var dto = await _catalogRepository.GetByUserId(id);
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


            await _catalogRepository.Create(dto);
            return dto is null
                ? NotFound<CatalogRegisterDto?>("Catálogo não encontrado para este usuário.")
                : Ok<CatalogRegisterDto?>(dto);
        }


    }
}
