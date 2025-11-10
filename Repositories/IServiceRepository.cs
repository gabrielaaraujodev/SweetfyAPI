namespace SweetfyAPI.Repositories
{
    public interface IServiceRepository
    {
        Task<IEnumerable<Service>> GetByBakeryIdAsync(int bakeryId);
        Task<Service?> GetByIdAsync(int id);
        Task<Service> AddAsync(Service service);
        Task<Service?> UpdateAsync(Service service);
        Task<Service?> DeleteAsync(int id);
    }

}
