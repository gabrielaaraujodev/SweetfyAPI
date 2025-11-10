namespace SweetfyAPI.DTOs.RecipeDTO
{
    public record CreateRecipeDto(
        string Name,
        decimal YieldQuantity,
        UnitType? YieldUnit,
        string? Preparation,
        decimal AdditionalCostPercent,
        List<CreateRecipeIngredientDto> RecipeIngredients,
        List<CreateRecipeServiceDto> RecipeServices
    );
}
