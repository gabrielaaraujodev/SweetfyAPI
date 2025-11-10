using Microsoft.EntityFrameworkCore;
using SweetfyAPI.Entities;
using SweetfyAPI.Repositories; 

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Order>> GetByBakeryIdAsync(int bakeryId)
    {
        return await _context.Orders
            .AsNoTracking()
            .Where(o => o.BakeryId == bakeryId)
            .ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Order?> GetByIdWithItemsAsync(int id)
    {
        return await _context.Orders
            .AsNoTracking()
            .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
            .Include(o => o.OrderRecipes)
                .ThenInclude(or => or.Recipe)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Order> AddAsync(Order order)
    {
        order.CreatedAt = DateTime.UtcNow;
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<Order?> UpdateAsync(Order order)
    {
        try
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }
        catch (DbUpdateConcurrencyException)
        {
            return null;
        }
    }

    public async Task<Order?> DeleteAsync(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
            return null;

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return order;
    }
}