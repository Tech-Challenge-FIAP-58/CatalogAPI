using AutoMapper;
using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Models.Cart;

namespace FCG.Catalog.Infra.Mapping;

public class CartProfile : Profile
{
    public CartProfile()
    {
        CreateMap<CartItem, CartItemResponseDto>()
            .ConstructUsing(item => new CartItemResponseDto(
                item.GameId,
                item.Name,
                item.Platform,
                item.PublisherName,
                item.Description,
                item.UnitPrice,
                item.Quantity,
                item.Total));

        CreateMap<Cart, CartResponseDto>()
            .ConstructUsing(cart => new CartResponseDto(
                cart.Id,
                cart.UserId,
                cart.Total,
                cart.Items.Select(item => new CartItemResponseDto(
                    item.GameId,
                    item.Name,
                    item.Platform,
                    item.PublisherName,
                    item.Description,
                    item.UnitPrice,
                    item.Quantity,
                    item.Total)).ToList(),
                cart.Status));
    }
}
