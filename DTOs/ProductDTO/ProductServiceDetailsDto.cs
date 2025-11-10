namespace SweetfyAPI.DTOs.ProductDTO
{
    public record ProductServiceDetailsDto(
        int Id,
        int ServiceId,
        string ServiceName,
        decimal Quantity,
        decimal? UnitPriceSnapshot
    );
}
