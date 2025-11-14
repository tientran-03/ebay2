using _30_TranVietTien.Models;
using _30_TranVietTien.Repositories;
using System;
using System.Threading.Tasks;

namespace _30_TranVietTien.Services
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _repo;
        public CouponService(ICouponRepository repo)
        {
            _repo = repo;
        }

        public async Task<decimal?> ApplyCouponAsync(string code, decimal totalPrice)
        {
            if (string.IsNullOrEmpty(code)) return null;

            var coupon = await _repo.GetByCodeAsync(code);
            if (coupon == null) return null;

            // Kiểm tra thời hạn
            if (coupon.StartDate > DateTime.Now || coupon.EndDate < DateTime.Now)
                return null;

            // Giới hạn sử dụng
            if (coupon.MaxUsage <= 0)
                return null;

            // Áp dụng giảm giá
            var discountAmount = totalPrice * (coupon.DiscountPercent ?? 0) / 100;

            // Giảm số lần sử dụng
            coupon.MaxUsage = (coupon.MaxUsage ?? 1) - 1;
            await _repo.UpdateAsync(coupon);

            return totalPrice - discountAmount;
        }
        public async Task<IEnumerable<Coupon>> GetSellerCouponsAsync(int sellerId)
     => await _repo.GetBySellerIdAsync(sellerId);

        public async Task CreateAsync(Coupon coupon)
            => await _repo.AddAsync(coupon);

        public async Task DeleteAsync(int id)
            => await _repo.DeleteAsync(id);
    }
}
