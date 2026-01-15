using AutoMapper;
using FCG.Core.Core.Inputs;
using FCG.Core.Core.Models;

namespace FCG.Infra.Mapping;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        // REGISTER: DTO -> Entity
        CreateMap<OrderRegisterDto, Order>()
            // Gerenciados pela aplicação/EF
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedAtUtc, opt => opt.Ignore());

        // UPDATE: DTO -> Entity (aplica apenas quando vier valor)
        CreateMap<OrderUpdateDto, Order>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedAtUtc, opt => opt.Ignore())
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        // Entity -> DTO de resposta
        CreateMap<Order, OrderResponseDto>();
    }


}
