namespace SweetfyAPI.DTOs.RecipeDTO
{
    public record RecipeIngredientDetailsDto(
        int Id, 
        int IngredientId,
        string IngredientName,
        decimal Quantity,
        UnitType Unit,
        decimal? UnitPriceSnapshot
    );
}
