namespace SweetfyAPI.DTOs.OrderDTO
{
    public record OrderDto(
        int Id,
        string Name,
        decimal? SalePrice,
        decimal? Profit,
        string Status,
        DateTime CreatedAt
    );  
}
