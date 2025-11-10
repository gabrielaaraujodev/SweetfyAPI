using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class RecipeService
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;

    [Required]
    public int ServiceId { get; set; }
    public Service Service { get; set; } = null!;

    [Required, Column(TypeName = "decimal(18,4)")]
    public decimal Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? UnitPriceSnapshot { get; set; }
}
