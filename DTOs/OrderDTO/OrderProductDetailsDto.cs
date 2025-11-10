namespace SweetfyAPI.DTOs.OrderDTO
{
    public record OrderProductDetailsDto(
        int Id,
        int ProductId,
        string ProductName,
        decimal Quantity,
        decimal? UnitPriceSnapshot,
        decimal? CostSnapshot
    );
}
