namespace SweetfyAPI.Repositories
{
    public interface IProductServiceRepository
    {
        Task<IEnumerable<ProductService>> GetByProductIdAsync(int productId);
        Task<ProductService?> GetByIdAsync(int id);
        Task<ProductService> AddAsync(ProductService productService);
        Task<ProductService?> UpdateAsync(ProductService productService);
        Task<ProductService?> DeleteAsync(int id);
    }

}
