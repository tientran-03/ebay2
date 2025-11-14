using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _30_TranVietTien.Models;
using _30_TranVietTien.Services;

namespace _30_TranVietTien.Controllers
{
    public class SellerCouponController : Controller
    {
        private readonly CloneEbayDbContext _context;
        private readonly IUserService _userService;

        public SellerCouponController(CloneEbayDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }
        public async Task<IActionResult> Index()
        {
            var me = await _userService.GetCurrentAsync(HttpContext);
            if (me == null)
                return RedirectToAction("Login", "Account");

            var coupons = await _context.Coupons
                .Include(c => c.Product)
                .Where(c => c.Product.SellerId == me.Id)
                .OrderByDescending(c => c.Id)
                .ToListAsync();

            return View(coupons);
        }

        [HttpGet]
        public async Task<IActionResult> GetCoupon(int id)
        {
            var c = await _context.Coupons.Include(x => x.Product).FirstOrDefaultAsync(x => x.Id == id);
            if (c == null) return Json(new { success = false });

            return Json(new
            {
                success = true,
                id = c.Id,
                code = c.Code,
                discountPercent = c.DiscountPercent,
                startDate = c.StartDate?.ToString("yyyy-MM-dd"),
                endDate = c.EndDate?.ToString("yyyy-MM-dd"),
                maxUsage = c.MaxUsage,
                productId = c.ProductId
            });
        }

        [HttpPost]
        public async Task<IActionResult> Save(Coupon coupon)
        {

            if (coupon.Id == 0)
                _context.Coupons.Add(coupon);
            else
                _context.Coupons.Update(coupon);

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var c = await _context.Coupons.FindAsync(id);
            if (c == null) return Json(new { success = false });

            _context.Coupons.Remove(c);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
    }
}
