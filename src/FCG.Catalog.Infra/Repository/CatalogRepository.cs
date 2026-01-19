using AutoMapper;
using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Infra.Context;

namespace FCG.Catalog.Infra.Repository
{
    public class CatalogRepository(ApplicationDbContext context, IMapper mapper) : EFRepository<Domain.Models.Catalog>(context), ICatalogRepository
    {
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<CatalogResponseDto>> GetAll()
        {
            var catalogs = await Get();
            return [.. catalogs.Select(u => _mapper.Map<CatalogResponseDto>(u))];
        }

        public async Task<CatalogResponseDto?> GetByUserId(int id)
        {
            var catalog = await Get(id);
            return catalog is null ? null : _mapper.Map<CatalogResponseDto>(catalog);
        }

        public async Task<bool> Create(CatalogRegisterDto dto)
        {
            // conferir
            var catalog = _mapper.Map<Domain.Models.Catalog>(dto);

            await Register(catalog);

            return true;
        }

    }
}
