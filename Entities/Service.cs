using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Service
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Name { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(200)]
    public string? ProviderName { get; set; }

    [Required]
    public UnitType Unit { get; set; }

    [Required, Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public int BakeryId { get; set; }
    public Bakery Bakery { get; set; } = null!;

    public ICollection<RecipeService> RecipeServices { get; set; } = new List<RecipeService>();
    public ICollection<ProductService> ProductServices { get; set; } = new List<ProductService>();
}
