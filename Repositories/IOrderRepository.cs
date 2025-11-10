namespace SweetfyAPI.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetByBakeryIdAsync(int bakeryId);
        Task<Order?> GetByIdAsync(int id);
        Task<Order?> GetByIdWithItemsAsync(int id); 
        Task<Order> AddAsync(Order order);
        Task<Order?> UpdateAsync(Order order);
        Task<Order?> DeleteAsync(int id);
    }

}
