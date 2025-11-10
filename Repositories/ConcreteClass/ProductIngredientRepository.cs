using Microsoft.EntityFrameworkCore;
using SweetfyAPI.Entities;
using SweetfyAPI.Repositories; 

public class ProductIngredientRepository : IProductIngredientRepository
{
    private readonly AppDbContext _context;

    public ProductIngredientRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductIngredient>> GetByProductIdAsync(int productId)
    {
        return await _context.ProductIngredients
            .Where(pi => pi.ProductId == productId)
            .Include(pi => pi.Ingredient)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<ProductIngredient?> GetByIdAsync(int id)
    {
        return await _context.ProductIngredients
            .Include(pi => pi.Ingredient)
            .AsNoTracking()
            .FirstOrDefaultAsync(pi => pi.Id == id);
    }

    public async Task<ProductIngredient> AddAsync(ProductIngredient productIngredient)
    {
        await _context.ProductIngredients.AddAsync(productIngredient);
        await _context.SaveChangesAsync();
        return productIngredient;
    }

    public async Task<ProductIngredient?> UpdateAsync(ProductIngredient productIngredient)
    {
        _context.ProductIngredients.Update(productIngredient);
        await _context.SaveChangesAsync();
        return productIngredient;
    }

    public async Task<ProductIngredient?> DeleteAsync(int id)
    {
        var productIngredient = await _context.ProductIngredients.FindAsync(id);
        if (productIngredient == null)
            return null;

        _context.ProductIngredients.Remove(productIngredient);
        await _context.SaveChangesAsync();
        return productIngredient;
    }
}