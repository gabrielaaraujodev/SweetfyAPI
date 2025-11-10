using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class OrderProduct
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    [Required]
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    [Required, Column(TypeName = "decimal(18,4)")]
    public decimal Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? UnitPriceSnapshot { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? CostSnapshot { get; set; }
}
