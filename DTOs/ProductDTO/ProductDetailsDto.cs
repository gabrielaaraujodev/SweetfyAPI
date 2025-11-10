namespace SweetfyAPI.DTOs.ProductDTO
{
    public record ProductDetailsDto(
       int Id,
       string Name,
       string? Preparation,
       decimal? BaseCost,
       decimal? SalePrice,
       decimal? ProfitPercent,
       decimal? ProfitAmount,
       DateTime CreatedAt,
       DateTime? UpdatedAt,
       List<ProductIngredientDetailsDto> ProductIngredients,
       List<ProductRecipeDetailsDto> ProductRecipes,
       List<ProductServiceDetailsDto> ProductServices
   );
}
