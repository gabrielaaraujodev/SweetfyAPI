namespace SweetfyAPI.Repositories
{
    public interface IOrderProductRepository
    {
        Task<IEnumerable<OrderProduct>> GetByOrderIdAsync(int orderId);
        Task<OrderProduct?> GetByIdAsync(int id);
        Task<OrderProduct> AddAsync(OrderProduct orderProduct);
        Task<OrderProduct?> UpdateAsync(OrderProduct orderProduct);
        Task<OrderProduct?> DeleteAsync(int id);
    }

}
