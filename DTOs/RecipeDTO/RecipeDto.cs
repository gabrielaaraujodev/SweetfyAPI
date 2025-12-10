namespace SweetfyAPI.DTOs.RecipeDTO
{
    public record RecipeDto(
        int Id,
        string Name,
        string Preparation,
        decimal YieldQuantity,
        decimal BaseCost,
        decimal AdditionalCostPercent,
        UnitType? YieldUnit,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        List<RecipeIngredientDetailsDto> RecipeIngredients,
        List<RecipeServiceDetailsDto> RecipeServices
    );
}
