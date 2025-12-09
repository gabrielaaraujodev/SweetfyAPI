using Microsoft.EntityFrameworkCore;
using SweetfyAPI.Entities;
using SweetfyAPI.Repositories;


public class IngredientRepository : IIngredientRepository
{
    private readonly AppDbContext _context;

    public IngredientRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Ingredient>> GetByBakeryIdAsync(int bakeryId)
    {
        return await _context.Ingredients
            .Where(i => i.BakeryId == bakeryId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Ingredient?> GetByIdAsync(int id)
    {
        return await _context.Ingredients.FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<Ingredient> AddAsync(Ingredient ingredient)
    {
        if (ingredient is null)
            throw new ArgumentNullException(nameof(ingredient));

        await _context.Ingredients.AddAsync(ingredient);
        await _context.SaveChangesAsync();
        return ingredient;
    }

    public async Task<Ingredient?> UpdateAsync(Ingredient ingredient)
    {
        if (ingredient is null)
            throw new ArgumentNullException(nameof(ingredient));

        _context.Ingredients.Update(ingredient);
        await _context.SaveChangesAsync();
        return ingredient;
    }

    public async Task<Ingredient?> DeleteAsync(int id)
    {
        var ingredient = await _context.Ingredients.FindAsync(id);
        if (ingredient is null)
            return null;

        _context.Ingredients.Remove(ingredient);
        await _context.SaveChangesAsync();
        return ingredient;
    }

    public async Task<List<Ingredient>> GetByIdsAsync(IEnumerable<int> ids)
    {
        return await _context.Ingredients.Where(s => ids.Contains(s.Id)).ToListAsync();
    }
    public async Task<bool> DeleteRangeAsync(IEnumerable<Ingredient> ingredients)
    {
        _context.Ingredients.RemoveRange(ingredients);
        return await _context.SaveChangesAsync() > 0;
    }
}