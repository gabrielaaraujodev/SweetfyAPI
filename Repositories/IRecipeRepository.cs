namespace SweetfyAPI.Repositories
{
    public interface IRecipeRepository
    {
        Task<IEnumerable<Recipe>> GetByBakeryIdAsync(int bakeryId);
        Task<Recipe?> GetByIdAsync(int id);
        Task<Recipe?> GetByIdWithComponentsAsync(int recipeId); 
        Task<Recipe> AddAsync(Recipe recipe);
        Task<Recipe?> UpdateAsync(Recipe recipe);
        Task<Recipe?> DeleteAsync(int id);

    }
}
