using AutoMapper;
using FCG.Core.Core.Inputs;
using FCG.Core.Core.Models.Entities;
using FCG.Infra.Context;

namespace FCG.Infra.Repository
{
    public class CatalogRepository(ApplicationDbContext context, IMapper mapper) : EFRepository<Catalog>(context), ICatalogRepository
    {
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<CatalogResponseDto>> GetAll()
        {
            var catalogs = await Get();
            return [.. catalogs.Select(u => _mapper.Map<CatalogResponseDto>(u))];
        }

        public async Task<CatalogResponseDto?> GetByUserId(Guid id)
        {
            var catalog = await Get(id);
            return catalog is null ? null : _mapper.Map<CatalogResponseDto>(catalog);
        }

        public async Task<bool> Create(CatalogRegisterDto dto)
        {
            // conferir
            var catalog = _mapper.Map<Catalog>(dto);

            await Register(catalog);

            return true;
        }

    }
}
