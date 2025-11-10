namespace SweetfyAPI.DTOs.ProductDTO
{
    public record ProductDto(
        int Id,
        string Name,
        decimal? BaseCost,
        decimal? SalePrice,
        decimal? ProfitAmount,
        DateTime CreatedAt,
        DateTime? UpdatedAt
    );
}
