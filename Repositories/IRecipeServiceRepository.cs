namespace SweetfyAPI.Repositories
{
    public interface IRecipeServiceRepository
    {
        Task<IEnumerable<RecipeService>> GetByRecipeIdAsync(int recipeId);
        Task<RecipeService?> GetByIdAsync(int id);
        Task<RecipeService> AddAsync(RecipeService recipeService);
        Task<RecipeService?> UpdateAsync(RecipeService recipeService);
        Task<RecipeService?> DeleteAsync(int id);
    }

}
