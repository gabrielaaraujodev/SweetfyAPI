using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SweetfyAPI.Entities;
using SweetfyAPI.Repositories.Interfaces; 


public class ApplicationUserRepository : IApplicationUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ApplicationUserRepository(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
    {
        return await _userManager.Users
            .Include(u => u.Bakery)
            .ToListAsync();
    }

    public async Task<ApplicationUser?> GetByIdAsync(string id)
    {
        return await _userManager.Users
            .Include(u => u.Bakery)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<ApplicationUser?> GetByEmailAsync(string email)
    {
        return await _userManager.Users
            .Include(u => u.Bakery)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> AddAsync(ApplicationUser user, string password)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        var result = await _userManager.CreateAsync(user, password);
        return result.Succeeded;
    }

    public async Task<bool> UpdateAsync(ApplicationUser user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return false; 

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }
}