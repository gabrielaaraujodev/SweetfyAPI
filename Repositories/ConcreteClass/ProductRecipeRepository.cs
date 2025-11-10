using Microsoft.EntityFrameworkCore;
using SweetfyAPI.Entities;
using SweetfyAPI.Repositories; 

public class ProductRecipeRepository : IProductRecipeRepository
{
    private readonly AppDbContext _context;

    public ProductRecipeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductRecipe>> GetByProductIdAsync(int productId)
    {
        return await _context.ProductRecipes
            .Where(pr => pr.ProductId == productId)
            .Include(pr => pr.Recipe)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<ProductRecipe?> GetByIdAsync(int id)
    {
        return await _context.ProductRecipes
            .Include(pr => pr.Recipe)
            .AsNoTracking()
            .FirstOrDefaultAsync(pr => pr.Id == id);
    }

    public async Task<ProductRecipe> AddAsync(ProductRecipe productRecipe)
    {
        await _context.ProductRecipes.AddAsync(productRecipe);
        await _context.SaveChangesAsync();
        return productRecipe;
    }

    public async Task<ProductRecipe?> UpdateAsync(ProductRecipe productRecipe)
    {
        _context.ProductRecipes.Update(productRecipe);
        await _context.SaveChangesAsync();
        return productRecipe;
    }

    public async Task<ProductRecipe?> DeleteAsync(int id)
    {
        var productRecipe = await _context.ProductRecipes.FindAsync(id);
        if (productRecipe == null)
            return null;

        _context.ProductRecipes.Remove(productRecipe);
        await _context.SaveChangesAsync();
        return productRecipe;
    }
}