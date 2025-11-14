using _30_TranVietTien.Models;
using _30_TranVietTien.Repositories;
using Microsoft.EntityFrameworkCore;

namespace _30_TranVietTien.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        private readonly IWebHostEnvironment _env;

        public ProductService(IProductRepository repo, IWebHostEnvironment env)
        {
            _repo = repo;
            _env = env;
        }
        public async Task<PagedList<Product>> GetPagedAsync(ProductFilter f)
        {
            var q = _repo.QueryActive().Where(p => p.IsAuction == true);


            if (f.CategoryId.HasValue)
                q = q.Where(p => p.CategoryId == f.CategoryId);

            if (!string.IsNullOrWhiteSpace(f.Q))
                q = q.Where(p => p.Title!.Contains(f.Q));

            if (f.Min.HasValue)
                q = q.Where(p => p.Price >= f.Min);

            if (f.Max.HasValue)
                q = q.Where(p => p.Price <= f.Max);

            q = f.Sort switch
            {
                "price_asc" => q.OrderBy(p => p.Price),
                "price_desc" => q.OrderByDescending(p => p.Price),
                "newest" => q.OrderByDescending(p => p.Id),
                _ => q.OrderBy(p => p.Id)
            };

            var total = await q.CountAsync();
            var items = await q.Skip((f.Page - 1) * f.PageSize)
                               .Take(f.PageSize)
                               .ToListAsync();

            return new PagedList<Product>(items, f.Page, f.PageSize, total);
        }

        public Task<Product?> GetDetailAsync(int id)
            => _repo.GetDetailAsync(id);

        public async Task<IEnumerable<Product>> GetSellerProductsAsync(int sellerId)
            => await _repo.GetAllBySellerAsync(sellerId);

        public async Task<Product?> GetByIdAsync(int id)
            => await _repo.GetByIdAsync(id);
        public async Task CreateAsync(Product product, IFormFile? image)
        {
            if (image != null)
            {
                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
                var uploadDir = Path.Combine(_env.WebRootPath, "uploads", "products");
                Directory.CreateDirectory(uploadDir);

                var filePath = Path.Combine(uploadDir, fileName);
                using var fs = new FileStream(filePath, FileMode.Create);
                await image.CopyToAsync(fs);

                product.Images = $"/uploads/products/{fileName}";
            }

            if ((product.IsAuction ?? false) && product.AuctionEndTime == null)
            {
                product.AuctionEndTime = DateTime.Now.AddDays(7);
            }
            product.AuctionEndTime = DateTime.Now.AddDays(7);

            await _repo.AddAsync(product);
        }
        public async Task UpdateAsync(Product product, IFormFile? image)
        {
            var existing = await _repo.GetByIdAsync(product.Id);
            if (existing == null) return;

            existing.Title = product.Title;
            existing.Description = product.Description;
            existing.Price = product.Price;
            existing.CategoryId = product.CategoryId;
            existing.IsAuction = product.IsAuction;
            existing.AuctionEndTime = product.AuctionEndTime;

            if (image != null)
            {
                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
                var uploadDir = Path.Combine(_env.WebRootPath, "uploads", "products");
                Directory.CreateDirectory(uploadDir);

                var filePath = Path.Combine(uploadDir, fileName);
                using var fs = new FileStream(filePath, FileMode.Create);
                await image.CopyToAsync(fs);

                existing.Images = $"/uploads/products/{fileName}";
            }

            await _repo.UpdateAsync(existing);
        }
        public async Task DeleteAsync(int id)
        {
            var product = await _repo.GetByIdAsync(id);
            if (product != null)
                await _repo.DeleteAsync(product);
        }
        public async Task HideAsync(int id, bool isHidden)
        {
            var product = await _repo.GetByIdAsync(id);
            if (product == null) return;
            product.IsAuction = !isHidden;
            await _repo.UpdateAsync(product);
        }
    }
}
