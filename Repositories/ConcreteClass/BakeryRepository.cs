using Microsoft.EntityFrameworkCore;
using SweetfyAPI.Entities;
using SweetfyAPI.Repositories;


public class BakeryRepository : IBakeryRepository
{
    private readonly AppDbContext _context;

    public BakeryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Bakery>> GetAllAsync()
    {
        return await _context.Bakeries.ToListAsync();
    }

    public async Task<Bakery?> GetByIdAsync(int id)
    {
        return await _context.Bakeries.FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Bakery> AddAsync(Bakery bakery)
    {
        if (bakery is null)
            throw new ArgumentNullException(nameof(bakery));

        await _context.Bakeries.AddAsync(bakery);
        await _context.SaveChangesAsync();
        return bakery; 
    }

    public async Task<Bakery?> UpdateAsync(Bakery bakery)
    {
        if (bakery is null)
            throw new ArgumentNullException(nameof(bakery));

        _context.Bakeries.Update(bakery);
        await _context.SaveChangesAsync();
        return bakery;

    }

    public async Task<Bakery?> DeleteAsync(int id)
    {
        var bakery = await _context.Bakeries.FindAsync(id);
        if (bakery is null)
            return null; 

        _context.Bakeries.Remove(bakery);
        await _context.SaveChangesAsync();
        return bakery;
    }
}