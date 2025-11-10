using SweetfyAPI.DTOs.RecipeDTO;

namespace SweetfyAPI.Services
{
    public interface IRecipeService
    {
        Task<IEnumerable<Recipe>> GetRecipesForUserAsync();
        Task<Recipe?> GetRecipeDetailsByIdForUserAsync(int id);
        Task<Recipe?> CreateRecipeAsync(CreateRecipeDto dto);
        Task<Recipe?> UpdateRecipeAsync(int id, UpdateRecipeDto dto);
        Task<bool> DeleteRecipeAsync(int id);
    }
}