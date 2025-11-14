using _30_TranVietTien.Models;

namespace _30_TranVietTien.Services
{
    public interface IProductService
    {
        Task<PagedList<Product>> GetPagedAsync(ProductFilter f);
        Task<Product?> GetDetailAsync(int id);
        Task<IEnumerable<Product>> GetSellerProductsAsync(int sellerId);
        Task<Product?> GetByIdAsync(int id);
        Task CreateAsync(Product product, IFormFile? image);
        Task UpdateAsync(Product product, IFormFile? image);
        Task DeleteAsync(int id);
        Task HideAsync(int id, bool isHidden);
    }
}
