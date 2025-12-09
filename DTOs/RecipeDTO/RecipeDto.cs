namespace SweetfyAPI.DTOs.RecipeDTO
{
    public record RecipeDto(
        int Id,
        string Name,
        decimal YieldQuantity,
        decimal BaseCost,
        UnitType? YieldUnit,
        DateTime CreatedAt,
        DateTime? UpdatedAt
    );
}
