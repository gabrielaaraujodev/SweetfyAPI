namespace SweetfyAPI.Repositories
{
    public interface IRecipeIngredientRepository
    {
        Task<IEnumerable<RecipeIngredient>> GetByRecipeIdAsync(int recipeId);
        Task<RecipeIngredient?> GetByIdAsync(int id);
        Task<RecipeIngredient> AddAsync(RecipeIngredient recipeIngredient);
        Task<RecipeIngredient?> UpdateAsync(RecipeIngredient recipeIngredient);
        Task<RecipeIngredient?> DeleteAsync(int id);
    }

}
