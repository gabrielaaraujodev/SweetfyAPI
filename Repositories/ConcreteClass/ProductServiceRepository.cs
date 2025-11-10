using Microsoft.EntityFrameworkCore;
using SweetfyAPI.Entities;
using SweetfyAPI.Repositories;

public class ProductServiceRepository : IProductServiceRepository
{
    private readonly AppDbContext _context;

    public ProductServiceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductService>> GetByProductIdAsync(int productId)
    {
        return await _context.ProductServices
            .Where(ps => ps.ProductId == productId)
            .Include(ps => ps.Service)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<ProductService?> GetByIdAsync(int id)
    {
        return await _context.ProductServices
            .Include(ps => ps.Service)
            .AsNoTracking()
            .FirstOrDefaultAsync(ps => ps.Id == id);
    }

    public async Task<ProductService> AddAsync(ProductService productService)
    {
        await _context.ProductServices.AddAsync(productService);
        await _context.SaveChangesAsync();
        return productService;
    }

    public async Task<ProductService?> UpdateAsync(ProductService productService)
    {
        _context.ProductServices.Update(productService);
        await _context.SaveChangesAsync();
        return productService;
    }

    public async Task<ProductService?> DeleteAsync(int id)
    {
        var productService = await _context.ProductServices.FindAsync(id);
        if (productService == null)
            return null;

        _context.ProductServices.Remove(productService);
        await _context.SaveChangesAsync();
        return productService;
    }
}