using AutoMapper;
using FIAP.FCG.CATALOG.Core.Inputs;
using FIAP.FCG.CATALOG.Core.Models;


namespace FIAP.FCG.CATALOG.Infra.Mapping;

public class CatalogProfile : Profile
{
    public CatalogProfile()
    {
        CreateMap<CatalogRegisterDto, Catalog>();

    }
}
