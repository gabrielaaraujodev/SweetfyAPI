using System.ComponentModel.DataAnnotations;

namespace SweetfyAPI.DTOs.ServiceDTO
{
    public record BulkUpdateServiceItemDto(
         [Required] int Id, 
         [Required] string Name,
         string? Description,
         string? ProviderName,
         [Required] UnitType Unit,
         [Required] decimal UnitPrice
     );
}
