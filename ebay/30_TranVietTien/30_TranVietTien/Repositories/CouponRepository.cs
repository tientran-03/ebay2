using _30_TranVietTien.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace _30_TranVietTien.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly CloneEbayDbContext _context;
        public CouponRepository(CloneEbayDbContext context)
        {
            _context = context;
        }

        public async Task<Coupon?> GetByCodeAsync(string code)
        {
            return await _context.Coupons
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.Code == code);
        }

        public async Task UpdateAsync(Coupon coupon)
        {
            _context.Coupons.Update(coupon);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Coupon>> GetBySellerIdAsync(int sellerId)
        => await _context.Coupons
            .Include(c => c.Product)
            .Where(c => c.Product.SellerId == sellerId)
            .ToListAsync();

        public async Task AddAsync(Coupon coupon)
        {
            await _context.Coupons.AddAsync(coupon);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var c = await _context.Coupons.FindAsync(id);
            if (c != null)
            {
                _context.Coupons.Remove(c);
                await _context.SaveChangesAsync();
            }
        }
    }
}
