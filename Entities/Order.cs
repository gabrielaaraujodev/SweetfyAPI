using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Order
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BakeryId { get; set; }
    public Bakery Bakery { get; set; } = null!;

    [Required, StringLength(200)]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal? TotalYield { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? TotalCost { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? SalePrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Profit { get; set; }

    [Required, StringLength(50)]
    public string Status { get; set; } = "Draft";

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
    public ICollection<OrderRecipe> OrderRecipes { get; set; } = new List<OrderRecipe>();
}
