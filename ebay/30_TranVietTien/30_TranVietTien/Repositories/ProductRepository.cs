using _30_TranVietTien.Models;
using Microsoft.EntityFrameworkCore;

namespace _30_TranVietTien.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly CloneEbayDbContext _ctx;
        public ProductRepository(CloneEbayDbContext ctx)
        {
            _ctx = ctx;
        }

        public IQueryable<Product> QueryActive()
            => _ctx.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .AsQueryable();

        public async Task<Product?> GetDetailAsync(int id)
            => await _ctx.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.Reviewer)
                .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<IEnumerable<Product>> GetAllBySellerAsync(int sellerId)
            => await _ctx.Products
                .Include(p => p.Category)
            .Include(i => i.Inventories)
                .Where(p => p.SellerId == sellerId)
                .ToListAsync();

        public async Task<Product?> GetByIdAsync(int id)
            => await _ctx.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .FirstOrDefaultAsync(p => p.Id == id);

        public async Task AddAsync(Product product)
        {
            await _ctx.Products.AddAsync(product);
            await _ctx.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _ctx.Products.Update(product);
            await _ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(Product product)
        {
            _ctx.Products.Remove(product);
            await _ctx.SaveChangesAsync();
        }
    }
}
