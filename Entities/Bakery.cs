using SweetfyAPI.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Bakery
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Name { get; set; } = null!;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    public ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
    public ICollection<Service> Services { get; set; } = new List<Service>();
    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
