using Microsoft.EntityFrameworkCore;
using SweetfyAPI.Entities;
using SweetfyAPI.Repositories; 

public class RecipeServiceRepository : IRecipeServiceRepository
{
    private readonly AppDbContext _context;

    public RecipeServiceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RecipeService>> GetByRecipeIdAsync(int recipeId)
    {
        return await _context.RecipeServices
            .Where(rs => rs.RecipeId == recipeId)
            .Include(rs => rs.Service)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<RecipeService?> GetByIdAsync(int id)
    {
        return await _context.RecipeServices
            .Include(rs => rs.Service)
            .AsNoTracking()
            .FirstOrDefaultAsync(rs => rs.Id == id);
    }

    public async Task<RecipeService> AddAsync(RecipeService recipeService)
    {
        await _context.RecipeServices.AddAsync(recipeService);
        await _context.SaveChangesAsync();
        return recipeService;
    }

    public async Task<RecipeService?> UpdateAsync(RecipeService recipeService)
    {
        _context.RecipeServices.Update(recipeService);
        await _context.SaveChangesAsync();
        return recipeService;
    }

    public async Task<RecipeService?> DeleteAsync(int id)
    {
        var recipeService = await _context.RecipeServices.FindAsync(id);
        if (recipeService == null)
            return null;

        _context.RecipeServices.Remove(recipeService);
        await _context.SaveChangesAsync();
        return recipeService;
    }
}