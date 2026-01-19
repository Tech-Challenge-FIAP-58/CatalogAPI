using AutoMapper;
using FCG.Catalog.Domain.Inputs;

namespace FCG.Catalog.Infra.Mapping;

public class CatalogProfile : Profile
{
    public CatalogProfile()
    {
        CreateMap<CatalogRegisterDto, Domain.Models.Catalog>();

    }
}
