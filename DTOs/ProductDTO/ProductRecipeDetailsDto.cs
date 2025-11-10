namespace SweetfyAPI.DTOs.ProductDTO
{
    public record ProductRecipeDetailsDto(
        int Id,
        int RecipeId,
        string RecipeName,
        decimal Quantity,
        decimal? UnitPriceSnapshot
    );
}
