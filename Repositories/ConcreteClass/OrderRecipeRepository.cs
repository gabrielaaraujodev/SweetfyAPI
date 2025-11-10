using Microsoft.EntityFrameworkCore;
using SweetfyAPI.Entities;
using SweetfyAPI.Repositories; 

public class OrderRecipeRepository : IOrderRecipeRepository
{
    private readonly AppDbContext _context;

    public OrderRecipeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OrderRecipe>> GetByOrderIdAsync(int orderId)
    {
        return await _context.OrderRecipes
            .Include(op => op.Recipe)
            .Where(op => op.OrderId == orderId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<OrderRecipe?> GetByIdAsync(int id)
    {
        return await _context.OrderRecipes
            .Include(op => op.Recipe)
            .AsNoTracking()
            .FirstOrDefaultAsync(op => op.Id == id);
    }

    public async Task<OrderRecipe> AddAsync(OrderRecipe orderRecipe)
    {
        await _context.OrderRecipes.AddAsync(orderRecipe);
        await _context.SaveChangesAsync();
        return orderRecipe;
    }

    public async Task<OrderRecipe?> UpdateAsync(OrderRecipe orderRecipe)
    {
        _context.OrderRecipes.Update(orderRecipe);
        await _context.SaveChangesAsync();
        return orderRecipe;
    }

    public async Task<OrderRecipe?> DeleteAsync(int id)
    {
        var orderRecipe = await _context.OrderRecipes.FindAsync(id);
        if (orderRecipe == null)
            return null;

        _context.OrderRecipes.Remove(orderRecipe);
        await _context.SaveChangesAsync();
        return orderRecipe;
    }
}