using Microsoft.EntityFrameworkCore;
using SweetfyAPI.Entities;
using SweetfyAPI.Repositories;

public class OrderProductRepository : IOrderProductRepository
{
    private readonly AppDbContext _context;

    public OrderProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OrderProduct>> GetByOrderIdAsync(int orderId)
    {
        return await _context.OrderProducts
            .Include(op => op.Product)
            .Where(op => op.OrderId == orderId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<OrderProduct?> GetByIdAsync(int id)
    {
        return await _context.OrderProducts
            .Include(op => op.Product)
            .AsNoTracking()
            .FirstOrDefaultAsync(op => op.Id == id);
    }

    public async Task<OrderProduct> AddAsync(OrderProduct orderProduct)
    {
        await _context.OrderProducts.AddAsync(orderProduct);
        await _context.SaveChangesAsync();
        return orderProduct;
    }

    public async Task<OrderProduct?> UpdateAsync(OrderProduct orderProduct)
    {
        _context.OrderProducts.Update(orderProduct);
        await _context.SaveChangesAsync();
        return orderProduct;
    }

    public async Task<OrderProduct?> DeleteAsync(int id)
    {
        var orderProduct = await _context.OrderProducts.FindAsync(id);
        if (orderProduct == null)
            return null;

        _context.OrderProducts.Remove(orderProduct);
        await _context.SaveChangesAsync();
        return orderProduct;
    }
}