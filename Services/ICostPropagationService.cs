namespace SweetfyAPI.Services
{
    public interface ICostPropagationService
    {
        Task PropagateIngredientChangesAsync(int ingredientId, int bakeryId);
        Task PropagateServiceChangesAsync(int serviceId, int bakeryId);
        Task PropagateRecipeChangesAsync(int recipeId, int bakeryId);
        Task PropagateProductChangesAsync(int productId, int bakeryId);
    }
}
