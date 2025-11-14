using _30_TranVietTien.Models;
using System.Threading.Tasks;

namespace _30_TranVietTien.Services
{
    public interface ICouponService
    {
        Task<decimal?> ApplyCouponAsync(string code, decimal totalPrice);
        Task<IEnumerable<Coupon>> GetSellerCouponsAsync(int sellerId);
        Task CreateAsync(Coupon coupon);
        Task DeleteAsync(int id);
    }
}
