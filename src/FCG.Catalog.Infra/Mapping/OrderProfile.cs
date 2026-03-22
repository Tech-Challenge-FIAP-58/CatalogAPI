using AutoMapper;
using FCG.Catalog.Domain.Enums;
using FCG.Catalog.Domain.Events;
using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Models.Order;
using OrderAggregate = global::FCG.Catalog.Domain.Models.Order.Order;

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

        // UPDATE: DTO -> Entity (aplica apenas quando vier valor)
        CreateMap<OrderUpdateDto, OrderAggregate>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<OrderItem, OrderItemSnapshot>()
            .ConstructUsing(item => new OrderItemSnapshot(
                item.GameId,
                item.Name,
                item.Platform,
                item.PublisherName,
                item.Description,
                item.UnitPrice));

        CreateMap<OrderAggregate, OrderResponseDto>()
            .ConstructUsing(order => new OrderResponseDto(
                order.Id,
                order.OrderDate,
                order.UserId,
                order.Total,
                order.Status,
                order.Items.Select(item => new OrderItemSnapshot(
                    item.GameId,
                    item.Name,
                    item.Platform,
                    item.PublisherName,
                    item.Description,
                    item.UnitPrice)).ToList()));
    }


}
