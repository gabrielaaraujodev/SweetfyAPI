using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class RecipeIngredient
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;

    [Required]
    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;

    [Required, Column(TypeName = "decimal(18,4)")]
    public decimal Quantity { get; set; }

    [Required]
    public UnitType Unit { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? UnitPriceSnapshot { get; set; }
}
