namespace SweetfyAPI.DTOs.ProductDTO
{
    public record CreateProductIngredientDto(
        int IngredientId,
        decimal Quantity,
        UnitType Unit
    );
}
