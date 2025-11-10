namespace SweetfyAPI.DTOs.ServiceDTO
{
    public record UpdateServiceDto(
        string Name,
        string? Description,
        string? ProviderName,
        UnitType Unit,
        decimal UnitPrice
    );
}
