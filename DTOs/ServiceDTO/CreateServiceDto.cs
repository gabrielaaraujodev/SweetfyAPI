namespace SweetfyAPI.DTOs.ServiceDTO
{
    public record CreateServiceDto(
        string Name,
        string? Description,
        string? ProviderName,
        UnitType Unit,
        decimal UnitPrice
    );
}
