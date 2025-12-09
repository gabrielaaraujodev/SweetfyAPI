using SweetfyAPI.DTOs.RecipeDTO;
using SweetfyAPI.DTOs.ServiceDTO;

namespace SweetfyAPI.Services
{
    public interface IRecipeService
    {
        Task<IEnumerable<Recipe>> GetRecipesForUserAsync();
        Task<Recipe?> GetRecipeDetailsByIdForUserAsync(int id);
        Task<Recipe?> CreateRecipeAsync(CreateRecipeDto dto);
        Task<Recipe?> UpdateRecipeAsync(int id, UpdateRecipeDto dto);
        Task<bool> DeleteRecipeAsync(int id);
        decimal CalculateRecipeCost(Recipe recipe);
        Task<(bool IsSuccess, string Message)> BulkDeleteRecipesAsync(IEnumerable<int> ids);
    }
}