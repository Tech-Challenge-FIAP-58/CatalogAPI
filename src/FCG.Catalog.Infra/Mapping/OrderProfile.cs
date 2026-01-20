using AutoMapper;
using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Models;

namespace FCG.Catalog.Infra.Mapping;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        // REGISTER: DTO -> Entity
        CreateMap<OrderRegisterDto, Order>()
            // Gerenciados pela aplicação/EF
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore());

        // UPDATE: DTO -> Entity (aplica apenas quando vier valor)
        CreateMap<OrderUpdateDto, Order>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        // Entity -> DTO de resposta
        CreateMap<Order, OrderResponseDto>();
    }


}
