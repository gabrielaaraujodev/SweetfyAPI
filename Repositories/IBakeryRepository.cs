namespace SweetfyAPI.Repositories
{
    public interface IBakeryRepository
    {
        Task<IEnumerable<Bakery>> GetAllAsync();
        Task<Bakery?> GetByIdAsync(int id);
        Task<Bakery> AddAsync(Bakery bakery);
        Task<Bakery?> UpdateAsync(Bakery bakery);
        Task<Bakery?> DeleteAsync(int id);
    }
}
