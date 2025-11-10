using Microsoft.EntityFrameworkCore;
using SweetfyAPI.Entities;
using SweetfyAPI.Repositories; 

public class ServiceRepository : IServiceRepository
{
    private readonly AppDbContext _context;

    public ServiceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Service>> GetByBakeryIdAsync(int bakeryId)
    {
        return await _context.Services
            .Where(s => s.BakeryId == bakeryId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Service?> GetByIdAsync(int id)
    {
        return await _context.Services.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Service> AddAsync(Service service)
    {
        await _context.Services.AddAsync(service);
        await _context.SaveChangesAsync();
        return service;
    }

    public async Task<Service?> UpdateAsync(Service service)
    {
        _context.Services.Update(service);
        await _context.SaveChangesAsync();
        return service;
    }

    public async Task<Service?> DeleteAsync(int id)
    {
        var service = await _context.Services.FindAsync(id);
        if (service == null)
            return null;

        _context.Services.Remove(service);
        await _context.SaveChangesAsync();
        return service;
    }
}