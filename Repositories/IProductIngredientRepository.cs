namespace SweetfyAPI.Repositories
{
    public interface IProductIngredientRepository
    {
        Task<IEnumerable<ProductIngredient>> GetByProductIdAsync(int productId);
        Task<ProductIngredient?> GetByIdAsync(int id);
        Task<ProductIngredient> AddAsync(ProductIngredient productIngredient);
        Task<ProductIngredient?> UpdateAsync(ProductIngredient productIngredient);
        Task<ProductIngredient?> DeleteAsync(int id);
    }

}
