using backend.Dto;

namespace backend.Interface
{
    public interface IShopDao
    {
        Task<List<ProductListItemDto>> GetAllProductsAsync();

        Task<BuyProductResultDto> BuyAsync(long userId, BuyProductRequestDto dto);

        Task<List<OrderSummaryDto>> GetUserOrdersAsync(long userId);
        Task<bool> UpdateOrderStatusAsync(long userId, long orderId, string status);

        Task<OrderDetailDto?> GetOrderDetailAsync(long userId, long orderId);
        Task<bool> UpdateShipmentStatusAsync(long userId, long orderId, string status);
    }
}
