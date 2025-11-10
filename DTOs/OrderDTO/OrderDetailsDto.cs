namespace SweetfyAPI.DTOs.OrderDTO
{
    public record OrderDetailsDto(
        int Id,
        string Name,
        string? Description,
        decimal? TotalYield,
        decimal? TotalCost,
        decimal? SalePrice,
        decimal? Profit,
        string Status,
        DateTime CreatedAt,
        List<OrderProductDetailsDto> OrderProducts,
        List<OrderRecipeDetailsDto> OrderRecipes
    );
}
