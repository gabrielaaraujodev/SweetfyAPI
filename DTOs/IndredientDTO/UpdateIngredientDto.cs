namespace SweetfyAPI.DTOs.IndredientDTO
{
    public record UpdateIngredientDto(
        string Name,
        string? Description,
        string? Brand,
        decimal Quantity,
        UnitType Unit,
        decimal UnitPrice
    );
}
