namespace SweetfyAPI.DTOs.ServiceDTO
{
    public record ServiceDto(
        int Id,
        string Name,
        string? Description,
        string? ProviderName,
        UnitType Unit,
        decimal UnitPrice,
        DateTime CreatedAt
    );
}
