namespace SweetfyAPI.DTOs.OrderDTO
{
    public record CreateOrderRecipeDto(
        int RecipeId,
        decimal Quantity
    );
}
