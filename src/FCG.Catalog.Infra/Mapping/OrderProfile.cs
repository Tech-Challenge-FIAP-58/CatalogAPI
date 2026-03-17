using AutoMapper;
using FCG.Catalog.Domain.Enums;
using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Models;

namespace FCG.Catalog.Infra.Mapping;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        // REGISTER: DTO -> Entity
        //CreateMap<OrderRegisterDto, Order>()
        //    // Gerenciados pela aplicação/EF
        //    .ForMember(d => d.Id, opt => opt.Ignore())
        //    .ForMember(d => d.CreatedAt, opt => opt.Ignore());

        CreateMap<OrderRegisterDto, Order>()
            .ConvertUsing((dto, _, context) =>
            {
                // Mapeia a lista de DTOs de jogos para a lista da entidade Game
                var games = context.Mapper.Map<List<Game>>(dto.OrderGames);

                return Order.Create(
                    dto.OrderDate,
                    dto.UserId,
                    dto.Total,
                    OrderStatus.Authorized,
                    games
                );
            });

        // UPDATE: DTO -> Entity (aplica apenas quando vier valor)
        CreateMap<OrderUpdateDto, Order>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        // Entity -> DTO de resposta
        CreateMap<Order, OrderResponseDto>();
    }


}
