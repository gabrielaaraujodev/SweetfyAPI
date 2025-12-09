using Microsoft.EntityFrameworkCore;
using SweetfyAPI.Entities;
using SweetfyAPI.Repositories; 

public class RecipeRepository : IRecipeRepository
{
    private readonly AppDbContext _context;

    public RecipeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Recipe>> GetByBakeryIdAsync(int bakeryId)
    {
        return await _context.Recipes
              .Include(r => r.RecipeIngredients)
                  .ThenInclude(ri => ri.Ingredient)
              .Include(r => r.RecipeServices)
                  .ThenInclude(rs => rs.Service)
              .Where(r => r.BakeryId == bakeryId)
              .AsNoTracking()
              .ToListAsync();
    }

    public async Task<Recipe?> GetByIdAsync(int id)
    {
        return await _context.Recipes.FirstOrDefaultAsync(r => r.Id == id);
    }
    public async Task<List<Recipe>> GetByIdsAsync(IEnumerable<int> ids)
    {
        return await _context.Recipes
            .Where(r => ids.Contains(r.Id))
            .ToListAsync();
    }
    public async Task<bool> DeleteRangeAsync(IEnumerable<Recipe> recipes)
    {
        _context.Recipes.RemoveRange(recipes);
        var changes = await _context.SaveChangesAsync();
        return changes > 0;
    }

    public async Task<Recipe?> GetByIdWithComponentsAsync(int recipeId)
    {
        return await _context.Recipes
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.RecipeServices)
                .ThenInclude(rs => rs.Service)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == recipeId);
    }

    public async Task<Recipe> AddAsync(Recipe recipe)
    {
        await _context.Recipes.AddAsync(recipe);
        await _context.SaveChangesAsync();
        return recipe;
    }

    public async Task<Recipe?> UpdateAsync(Recipe recipe)
    {
        _context.Recipes.Update(recipe);
        await _context.SaveChangesAsync();
        return recipe;
    }

    public async Task<Recipe?> DeleteAsync(int id)
    {
        var recipe = await _context.Recipes.FindAsync(id);
        if (recipe == null)
            return null;

        _context.Recipes.Remove(recipe);
        await _context.SaveChangesAsync();
        return recipe;
    }
}