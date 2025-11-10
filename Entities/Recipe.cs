using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Recipe
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Name { get; set; } = null!;

    [Required, Column(TypeName = "decimal(18,4)")]
    public decimal YieldQuantity { get; set; } = 1;

    public UnitType? YieldUnit { get; set; }

    public string? Preparation { get; set; }

    [Required, Column(TypeName = "decimal(5,2)")]
    public decimal AdditionalCostPercent { get; set; } = 0;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [Required]
    public int BakeryId { get; set; }
    public Bakery Bakery { get; set; } = null!;
    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
    public ICollection<RecipeService> RecipeServices { get; set; } = new List<RecipeService>();
    public ICollection<ProductRecipe> ProductRecipes { get; set; } = new List<ProductRecipe>();
    public ICollection<OrderRecipe> OrderRecipes { get; set; } = new List<OrderRecipe>();

}
