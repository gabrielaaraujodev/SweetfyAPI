namespace SweetfyAPI.DTOs.OrderDTO
{
    public record UpdateOrderDto(
        string Name,
        string? Description,
        string Status
    );
}
