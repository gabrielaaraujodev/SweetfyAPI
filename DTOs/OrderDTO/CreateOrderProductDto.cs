namespace SweetfyAPI.DTOs.OrderDTO
{
    public record CreateOrderProductDto(
        int ProductId,
        decimal Quantity
    );
}
