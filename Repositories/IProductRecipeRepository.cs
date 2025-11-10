namespace SweetfyAPI.Repositories
{
    public interface IProductRecipeRepository
    {
        Task<IEnumerable<ProductRecipe>> GetByProductIdAsync(int productId);
        Task<ProductRecipe?> GetByIdAsync(int id);
        Task<ProductRecipe> AddAsync(ProductRecipe productRecipe);
        Task<ProductRecipe?> UpdateAsync(ProductRecipe productRecipe);
        Task<ProductRecipe?> DeleteAsync(int id);
    }

}
