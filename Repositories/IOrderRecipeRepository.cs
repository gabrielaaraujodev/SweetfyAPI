namespace SweetfyAPI.Repositories
{
    public interface IOrderRecipeRepository
    {
        Task<IEnumerable<OrderRecipe>> GetByOrderIdAsync(int orderId);
        Task<OrderRecipe?> GetByIdAsync(int id);
        Task<OrderRecipe> AddAsync(OrderRecipe orderRecipe);
        Task<OrderRecipe?> UpdateAsync(OrderRecipe orderRecipe);
        Task<OrderRecipe?> DeleteAsync(int id);
    }

}
