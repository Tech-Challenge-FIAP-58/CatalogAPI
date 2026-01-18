using AutoMapper;
using FCG.Core.Core.Inputs;
using FCG.Core.Core.Models.Entities;

namespace FCG.Infra.Mapping;

public class CatalogProfile : Profile
{
    public CatalogProfile()
    {
        CreateMap<CatalogRegisterDto, Catalog>();

    }
}
