using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SweetfyAPI.Entities;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Bakery> Bakeries { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
    public DbSet<RecipeService> RecipeServices { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductIngredient> ProductIngredients { get; set; }
    public DbSet<ProductRecipe> ProductRecipes { get; set; }
    public DbSet<ProductService> ProductServices { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderProduct> OrderProducts { get; set; }
    public DbSet<OrderRecipe> OrderRecipes { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<UnitType>()
            .HaveConversion<string>();

        configurationBuilder
            .Properties<UnitType?>()
            .HaveConversion<string>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Model.GetEntityTypes()
            .SelectMany(e => e.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?))
            .ToList()
            .ForEach(property =>
            {
                property.SetColumnType("decimal(18, 2)");
            });

        modelBuilder.Entity<Ingredient>().Property(i => i.Quantity).HasColumnType("decimal(18, 4)");
        modelBuilder.Entity<Order>().Property(o => o.TotalYield).HasColumnType("decimal(18, 4)");
        modelBuilder.Entity<OrderProduct>().Property(op => op.Quantity).HasColumnType("decimal(18, 4)");
        modelBuilder.Entity<OrderRecipe>().Property(or => or.Quantity).HasColumnType("decimal(18, 4)");
        modelBuilder.Entity<ProductIngredient>().Property(pi => pi.Quantity).HasColumnType("decimal(18, 4)");
        modelBuilder.Entity<ProductRecipe>().Property(pr => pr.Quantity).HasColumnType("decimal(18, 4)");
        modelBuilder.Entity<ProductService>().Property(ps => ps.Quantity).HasColumnType("decimal(18, 4)");
        modelBuilder.Entity<Recipe>().Property(r => r.YieldQuantity).HasColumnType("decimal(18, 4)");
        modelBuilder.Entity<RecipeIngredient>().Property(ri => ri.Quantity).HasColumnType("decimal(18, 4)");
        modelBuilder.Entity<RecipeService>().Property(rs => rs.Quantity).HasColumnType("decimal(18, 4)");


        modelBuilder.Entity<ApplicationUser>()
            .HasOne(u => u.Bakery)
            .WithMany(b => b.Users)
            .HasForeignKey(u => u.BakeryId);

        modelBuilder.Entity<OrderProduct>()
            .HasOne(op => op.Product)
            .WithMany(p => p.OrderProducts)
            .HasForeignKey(op => op.ProductId)
            .OnDelete(DeleteBehavior.Restrict); 

        modelBuilder.Entity<OrderRecipe>()
            .HasOne(or => or.Recipe)
            .WithMany(r => r.OrderRecipes)
            .HasForeignKey(or => or.RecipeId)
            .OnDelete(DeleteBehavior.Restrict); 

        modelBuilder.Entity<ProductIngredient>()
            .HasOne(pi => pi.Ingredient)
            .WithMany(i => i.ProductIngredients)
            .HasForeignKey(pi => pi.IngredientId)
            .OnDelete(DeleteBehavior.Restrict); 

        modelBuilder.Entity<ProductRecipe>()
            .HasOne(pr => pr.Recipe)
            .WithMany(r => r.ProductRecipes)
            .HasForeignKey(pr => pr.RecipeId)
            .OnDelete(DeleteBehavior.Restrict); 

        modelBuilder.Entity<ProductService>()
            .HasOne(ps => ps.Service)
            .WithMany(s => s.ProductServices)
            .HasForeignKey(ps => ps.ServiceId)
            .OnDelete(DeleteBehavior.Restrict); 

        modelBuilder.Entity<RecipeIngredient>()
            .HasOne(ri => ri.Ingredient)
            .WithMany(i => i.RecipeIngredients)
            .HasForeignKey(ri => ri.IngredientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RecipeService>()
            .HasOne(rs => rs.Service)
            .WithMany(s => s.RecipeServices)
            .HasForeignKey(rs => rs.ServiceId)
            .OnDelete(DeleteBehavior.Restrict); 

        modelBuilder.Entity<Bakery>()
            .HasMany(b => b.Ingredients)
            .WithOne(i => i.Bakery)
            .HasForeignKey(i => i.BakeryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Bakery>()
            .HasMany(b => b.Services)
            .WithOne(s => s.Bakery)
            .HasForeignKey(s => s.BakeryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Bakery>()
            .HasMany(b => b.Recipes)
            .WithOne(r => r.Bakery)
            .HasForeignKey(r => r.BakeryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Bakery>()
            .HasMany(b => b.Products)
            .WithOne(p => p.Bakery)
            .HasForeignKey(p => p.BakeryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Bakery>()
            .HasMany(b => b.Orders)
            .WithOne(o => o.Bakery)
            .HasForeignKey(o => o.BakeryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Recipe>()
            .HasMany(r => r.RecipeIngredients)
            .WithOne(ri => ri.Recipe)
            .HasForeignKey(ri => ri.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Recipe>()
            .HasMany(r => r.RecipeServices)
            .WithOne(rs => rs.Recipe)
            .HasForeignKey(rs => rs.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Product>()
            .HasMany(p => p.ProductIngredients)
            .WithOne(pi => pi.Product)
            .HasForeignKey(pi => pi.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Product>()
            .HasMany(p => p.ProductRecipes)
            .WithOne(pr => pr.Product)
            .HasForeignKey(pr => pr.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Product>()
            .HasMany(p => p.ProductServices)
            .WithOne(ps => ps.Product)
            .HasForeignKey(ps => ps.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Order>()
            .HasMany(o => o.OrderProducts)
            .WithOne(op => op.Order)
            .HasForeignKey(op => op.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Order>()
            .HasMany(o => o.OrderRecipes)
            .WithOne(or => or.Order)
            .HasForeignKey(or => or.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}