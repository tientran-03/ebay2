using Microsoft.AspNetCore.Mvc;
using _30_TranVietTien.Models;
using _30_TranVietTien.Services;

public class CouponController : Controller
{
    private readonly ICouponService _couponService;
    private readonly IUserService _userService;

    public CouponController(ICouponService couponService, IUserService userService)
    {
        _couponService = couponService;
        _userService = userService;
    }

    public async Task<IActionResult> Index()
    {
        var me = await _userService.GetCurrentAsync(HttpContext);
        if (me == null) return RedirectToAction("Login", "Account");

        var coupons = await _couponService.GetSellerCouponsAsync(me.Id);
        return View(coupons);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Coupon coupon)
    {
        await _couponService.CreateAsync(coupon);
        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _couponService.DeleteAsync(id);
        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Apply(string code, decimal totalPrice)
    {
        var result = await _couponService.ApplyCouponAsync(code, totalPrice);
        if (result == null)
            return Json(new { success = false, message = "Mã không hợp lệ hoặc đã hết hạn." });

        return Json(new { success = true, newTotal = result });
    }
}
