namespace SweetfyAPI.DTOs.RecipeDTO
{
    public record RecipeDto(
        int Id,
        string Name,
        decimal YieldQuantity,
        UnitType? YieldUnit,
        DateTime CreatedAt,
        DateTime? UpdatedAt
    );
}
