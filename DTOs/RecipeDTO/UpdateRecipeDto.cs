namespace SweetfyAPI.DTOs.RecipeDTO
{
    public record UpdateRecipeDto(
        string Name,
        decimal YieldQuantity,
        UnitType? YieldUnit,
        string? Preparation,
        decimal AdditionalCostPercent,
        List<CreateRecipeIngredientDto> RecipeIngredients,
        List<CreateRecipeServiceDto> RecipeServices
    );
}
