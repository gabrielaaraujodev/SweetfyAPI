namespace SweetfyAPI.DTOs.RecipeDTO
{
    public record CreateRecipeIngredientDto(
        int IngredientId,
        decimal Quantity,
        UnitType Unit
    );
}
