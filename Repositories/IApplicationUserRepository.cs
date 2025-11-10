using SweetfyAPI.Entities;

namespace SweetfyAPI.Repositories.Interfaces
{
    public interface IApplicationUserRepository
    {
        Task<IEnumerable<ApplicationUser>> GetAllAsync();
        Task<ApplicationUser?> GetByIdAsync(string id);
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<bool> AddAsync(ApplicationUser user, string password);
        Task<bool> UpdateAsync(ApplicationUser user);
        Task<bool> DeleteAsync(string id);
    }
}
