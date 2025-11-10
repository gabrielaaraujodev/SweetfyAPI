
using SweetfyAPI.Entities;

namespace SweetfyAPI.Services
{
    public interface IUserService
    {
        int GetMyBakeryId();

        Task<ApplicationUser?> GetMeAsync();
    }
}