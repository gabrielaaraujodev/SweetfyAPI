using SweetfyAPI.DTOs.OrderDTO;

namespace SweetfyAPI.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetOrdersForUserAsync();
        Task<Order?> GetOrderDetailsByIdForUserAsync(int id);
        Task<Order?> CreateOrderAsync(CreateOrderDto dto);
        Task<Order?> UpdateOrderAsync(int id, UpdateOrderDto dto); 
        Task<bool> DeleteOrderAsync(int id);
    }
}