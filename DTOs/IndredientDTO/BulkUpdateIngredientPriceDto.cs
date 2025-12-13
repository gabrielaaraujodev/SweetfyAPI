using System.ComponentModel.DataAnnotations;

namespace SweetfyAPI.DTOs.IndredientDTO
{
    public record BulkUpdateIngredientPriceDto(
         [Required] int Id,
         [Required] decimal NewPrice 
     );
}
