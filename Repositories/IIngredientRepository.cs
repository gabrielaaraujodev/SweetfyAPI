namespace SweetfyAPI.Repositories
{
    public interface IIngredientRepository
    {
        Task<IEnumerable<Ingredient>> GetByBakeryIdAsync(int bakeryId);
        Task<Ingredient?> GetByIdAsync(int id);
        Task<Ingredient> AddAsync(Ingredient ingredient);
        Task<Ingredient?> UpdateAsync(Ingredient ingredient);
        Task<Ingredient?> DeleteAsync(int id);
    }

}
