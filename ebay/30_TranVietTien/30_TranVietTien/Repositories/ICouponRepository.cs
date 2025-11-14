using _30_TranVietTien.Models;
using System.Threading.Tasks;

namespace _30_TranVietTien.Repositories
{
    public interface ICouponRepository
    {
        Task<Coupon?> GetByCodeAsync(string code);
        Task UpdateAsync(Coupon coupon);
        Task<IEnumerable<Coupon>> GetBySellerIdAsync(int sellerId);
        Task AddAsync(Coupon coupon);
        Task DeleteAsync(int id);
    }
}
