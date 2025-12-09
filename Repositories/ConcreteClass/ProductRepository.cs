using Microsoft.EntityFrameworkCore;
using SweetfyAPI.Entities;
using SweetfyAPI.Repositories; 

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetByBakeryIdAsync(int bakeryId)
    {
        return await _context.Products
            .Include(p => p.ProductIngredients)
                .ThenInclude(pi => pi.Ingredient)
            .Include(p => p.ProductRecipes)
                .ThenInclude(pr => pr.Recipe)
            .Include(p => p.ProductServices)
                .ThenInclude(ps => ps.Service)
            .Where(p => p.BakeryId == bakeryId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Product>> GetByIdsAsync(IEnumerable<int> ids)
    {
        return await _context.Products
            .Where(r => ids.Contains(r.Id))
            .ToListAsync();
    }

    public async Task<bool> DeleteRangeAsync(IEnumerable<Product> products)
    {
        _context.Products.RemoveRange(products);
        var changes = await _context.SaveChangesAsync();
        return changes > 0;
    }
    public async Task<Product?> GetByIdWithComponentsAsync(int id)
    {
        return await _context.Products
            .Include(p => p.ProductIngredients)
                .ThenInclude(pi => pi.Ingredient)
            .Include(p => p.ProductRecipes)
                .ThenInclude(pr => pr.Recipe)
            .Include(p => p.ProductServices)
                .ThenInclude(ps => ps.Service)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product> AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product?> UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product?> DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return null;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return product;
    }
}