namespace SweetfyAPI.DTOs.ProductDTO
{
    public record UpdateProductDto(
        string Name,
        string? Preparation,
        decimal? SalePrice,
        decimal? ProfitPercent,
        List<CreateProductIngredientDto> ProductIngredients,
        List<CreateProductRecipeDto> ProductRecipes,
        List<CreateProductServiceDto> ProductServices
    );
}
