namespace SweetfyAPI.DTOs.RecipeDTO
{
    public record RecipeDetailsDto(
        int Id,
        string Name,
        decimal YieldQuantity,
        UnitType? YieldUnit,
        string? Preparation,
        decimal AdditionalCostPercent,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        List<RecipeIngredientDetailsDto> RecipeIngredients,
        List<RecipeServiceDetailsDto> RecipeServices
    );
}
