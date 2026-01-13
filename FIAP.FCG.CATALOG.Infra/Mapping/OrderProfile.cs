using AutoMapper;
using FIAP.FCG.CATALOG.Core.Inputs;
using FIAP.FCG.CATALOG.Core.Models;

namespace FIAP.FCG.Infra.Mapping;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        // REGISTER: DTO -> Entity
        CreateMap<OrderRegisterDto, Order>()
            // Gerenciados pela aplicação/EF
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedAtUtc, opt => opt.Ignore());
        

    }
}
