using SweetfyAPI.DTOs.ServiceDTO;

namespace SweetfyAPI.Services
{
    public interface IServiceService
    {
        Task<IEnumerable<Service>> GetServicesForUserAsync();
        Task<Service?> GetServiceByIdForUserAsync(int id);
        Task<Service> CreateServiceAsync(CreateServiceDto dto);
        Task<Service?> UpdateServiceAsync(int id, UpdateServiceDto dto);
        Task<bool> DeleteServiceAsync(int id);
    }
}