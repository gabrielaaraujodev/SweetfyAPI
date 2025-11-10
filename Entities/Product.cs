using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BakeryId { get; set; }
    public Bakery Bakery { get; set; } = null!;

    [Required, StringLength(200)]
    public string Name { get; set; } = null!;

    public string? Preparation { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? BaseCost { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? SalePrice { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal? ProfitPercent { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? ProfitAmount { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ICollection<ProductIngredient> ProductIngredients { get; set; } = new List<ProductIngredient>();
    public ICollection<ProductRecipe> ProductRecipes { get; set; } = new List<ProductRecipe>();
    public ICollection<ProductService> ProductServices { get; set; } = new List<ProductService>();
    public ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
}
