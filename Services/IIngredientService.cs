using SweetfyAPI.DTOs.IndredientDTO;

namespace SweetfyAPI.Services
{
    public interface IIngredientService
    {
        Task<IEnumerable<Ingredient>> GetIngredientsForUserAsync();
        Task<Ingredient?> GetIngredientByIdForUserAsync(int id);
        Task<Ingredient> CreateIngredientAsync(CreateIngredientDto dto);
        Task<Ingredient?> UpdateIngredientAsync(int id, UpdateIngredientDto dto);
        Task<bool> DeleteIngredientAsync(int id);
        Task<(bool IsSuccess, string Message)> BulkUpdateIngredientsAsync(List<BulkUpdateIngredientItemDTO> updates);
        Task<(bool IsSuccess, string Message)> BulkDeleteIngredientsAsync(IEnumerable<int> ids);

    }
}