namespace SweetfyAPI.DTOs.ProductDTO
{
    public record ProductIngredientDetailsDto(
        int Id,
        int IngredientId,
        string IngredientName,
        decimal Quantity,
        UnitType Unit,
        decimal? UnitPriceSnapshot
    );
}
