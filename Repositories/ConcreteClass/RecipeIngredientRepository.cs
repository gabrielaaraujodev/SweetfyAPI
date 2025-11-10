using Microsoft.EntityFrameworkCore;
using SweetfyAPI.Entities;
using SweetfyAPI.Repositories; 

public class RecipeIngredientRepository : IRecipeIngredientRepository
{
    private readonly AppDbContext _context;

    public RecipeIngredientRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RecipeIngredient>> GetByRecipeIdAsync(int recipeId)
    {
        return await _context.RecipeIngredients
            .Where(ri => ri.RecipeId == recipeId)
            .Include(ri => ri.Ingredient)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<RecipeIngredient?> GetByIdAsync(int id)
    {
        return await _context.RecipeIngredients
            .Include(ri => ri.Ingredient)
            .AsNoTracking()
            .FirstOrDefaultAsync(ri => ri.Id == id);
    }

    public async Task<RecipeIngredient> AddAsync(RecipeIngredient recipeIngredient)
    {
        await _context.RecipeIngredients.AddAsync(recipeIngredient);
        await _context.SaveChangesAsync();
        return recipeIngredient;
    }

    public async Task<RecipeIngredient?> UpdateAsync(RecipeIngredient recipeIngredient)
    {
        _context.RecipeIngredients.Update(recipeIngredient);
        await _context.SaveChangesAsync();
        return recipeIngredient;
    }

    public async Task<RecipeIngredient?> DeleteAsync(int id)
    {
        var recipeIngredient = await _context.RecipeIngredients.FindAsync(id);
        if (recipeIngredient == null)
            return null;

        _context.RecipeIngredients.Remove(recipeIngredient);
        await _context.SaveChangesAsync();
        return recipeIngredient;
    }
}