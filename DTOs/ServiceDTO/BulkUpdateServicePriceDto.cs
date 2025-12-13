using System.ComponentModel.DataAnnotations;

namespace SweetfyAPI.DTOs.ServiceDTO
{
    public record BulkUpdateServicePriceDto(
         [Required] int Id,
         [Required] decimal NewPrice
     );
}
