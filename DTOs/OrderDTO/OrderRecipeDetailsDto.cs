namespace SweetfyAPI.DTOs.OrderDTO
{
    public record OrderRecipeDetailsDto(
        int Id,
        int RecipeId,
        string RecipeName,
        decimal Quantity,
        decimal? UnitPriceSnapshot,
        decimal? CostSnapshot
    );
}
