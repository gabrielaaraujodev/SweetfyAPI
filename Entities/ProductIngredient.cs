using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ProductIngredient
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

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
