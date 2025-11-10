namespace SweetfyAPI.DTOs.OrderDTO
{
    public record CreateOrderDto(
        string Name,
        string? Description,
        string Status,
        List<CreateOrderProductDto> OrderProducts,
        List<CreateOrderRecipeDto> OrderRecipes
    );
}
