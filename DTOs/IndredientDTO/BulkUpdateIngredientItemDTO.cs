using System.ComponentModel.DataAnnotations;

namespace SweetfyAPI.DTOs.IndredientDTO
{
    public record BulkUpdateIngredientItemDTO(
     [Required] int Id, 
     string Name,
     string? Description,
     string? Brand,
     decimal Quantity,
     UnitType Unit,
     decimal UnitPrice
 );
}
