using SweetfyAPI.DTOs.ProductDTO;

namespace SweetfyAPI.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetProductsForUserAsync();
        Task<Product?> GetProductDetailsByIdForUserAsync(int id);
        Task<Product?> CreateProductAsync(CreateProductDto dto);
        Task<Product?> UpdateProductAsync(int id, UpdateProductDto dto);
        Task<bool> DeleteProductAsync(int id);
    }
}