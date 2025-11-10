namespace SweetfyAPI.DTOs.IndredientDTO
{
    public record CreateIngredientDto(
        string Name,
        string? Description,
        string? Brand,
        decimal Quantity,
        UnitType Unit,
        decimal UnitPrice
    );
}
