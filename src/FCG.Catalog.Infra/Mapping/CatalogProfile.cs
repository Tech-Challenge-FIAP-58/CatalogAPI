using AutoMapper;
using FCG.Catalog.Domain.Inputs;

namespace FCG.Infra.Mapping;

public class CatalogProfile : Profile
{
    public CatalogProfile()
    {
        CreateMap<CatalogRegisterDto, Catalog.Domain.Models.Catalog>();

    }
}
