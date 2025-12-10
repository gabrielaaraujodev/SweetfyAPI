namespace SweetfyAPI.DTOs.OrderDTO
{
    public record OrderDto(
        int Id,
        string Name,
        decimal? SalePrice,
        decimal? Profit,
        decimal? TotalCost,
        decimal? TotalYield,
        string Status,
        string Description,
        DateTime CreatedAt,
        List<OrderProductDetailsDto> OrderProducts
    );  
}
