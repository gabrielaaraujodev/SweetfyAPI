namespace SweetfyAPI.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetByBakeryIdAsync(int bakeryId);
        Task<Product?> GetByIdAsync(int id);
        Task<Product?> GetByIdWithComponentsAsync(int id); 
        Task<Product> AddAsync(Product product);
        Task<Product?> UpdateAsync(Product product);
        Task<Product?> DeleteAsync(int id);
    }

}
