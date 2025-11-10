namespace SweetfyAPI.DTOs.IndredientDTO
{
    public record IngredientDto(
        int Id,
        string Name,
        string? Description,
        string? Brand,
        decimal Quantity,
        UnitType Unit,
        decimal UnitPrice,
        DateTime CreatedAt,
        DateTime? UpdatedAt
    );
}
