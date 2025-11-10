namespace SweetfyAPI.DTOs.ProductDTO
{
    public record CreateProductDto(
        string Name,
        string? Preparation,
        decimal? SalePrice,
        decimal? ProfitPercent,
        List<CreateProductIngredientDto> ProductIngredients,
        List<CreateProductRecipeDto> ProductRecipes,
        List<CreateProductServiceDto> ProductServices
    );
}
