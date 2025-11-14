using _30_TranVietTien.Models;

namespace _30_TranVietTien.Repositories
{
    public interface IProductRepository
    {
        IQueryable<Product> QueryActive();
        Task<Product?> GetByIdAsync(int id);
        Task<Product?> GetDetailAsync(int id);
        Task<IEnumerable<Product>> GetAllBySellerAsync(int sellerId);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
    }
}
