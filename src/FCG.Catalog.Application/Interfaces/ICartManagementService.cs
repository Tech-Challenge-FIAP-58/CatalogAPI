using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Web;

namespace FCG.Catalog.Application.Interfaces
{
    public interface ICartManagementService
    {
        Task<IApiResponse<CartResponseDto?>> AddItem(CartAddItemDto dto);
        Task<IApiResponse<CartResponseDto?>> RemoveItem(CartRemoveItemDto dto);
        Task<IApiResponse<CartResponseDto?>> Clear(int userId);
        Task<IApiResponse<Guid?>> Checkout(CheckoutCartDto dto);
    }
}
