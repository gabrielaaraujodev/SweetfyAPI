namespace SweetfyAPI.DTOs.RecipeDTO
{
    public record RecipeDetailsDto(
        int Id,
        string Name,
        decimal YieldQuantity,
        decimal BaseCost,              
        string? Preparation,
        decimal AdditionalCostPercent,
        UnitType? YieldUnit,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        List<RecipeIngredientDetailsDto> RecipeIngredients,
        List<RecipeServiceDetailsDto> RecipeServices
    );
}
