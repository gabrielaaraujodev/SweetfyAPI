namespace SweetfyAPI.DTOs.RecipeDTO
{
    public record RecipeServiceDetailsDto(
        int Id, 
        int ServiceId,
        string ServiceName, 
        decimal Quantity,
        decimal? UnitPriceSnapshot
    );
}
