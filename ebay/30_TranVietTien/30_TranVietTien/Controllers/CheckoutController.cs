using _30_TranVietTien.Models;
using _30_TranVietTien.Services;
using Microsoft.AspNetCore.Mvc;

namespace _30_TranVietTien.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IUserService _user;
        private readonly ICartService _cart;
        private readonly IAddressService _addr;
        private readonly IOrderService _order;

        public CheckoutController(IUserService user, ICartService cart, IAddressService addr, IOrderService order)
        {
            _user = user;
            _cart = cart;
            _addr = addr;
            _order = order;
        }

        public async Task<IActionResult> Index()
        {
            var me = await _user.GetCurrentAsync(HttpContext);
            if (me == null)
                return RedirectToAction("Login", "Account");

            ViewBag.Addresses = await _addr.GetByUserAsync(me.Id);
            var cartItems = await _cart.GetAsync(HttpContext);

            return View(cartItems);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(int addressId, string method = "COD", string? couponCode = null)
        {
            var me = await _user.GetCurrentAsync(HttpContext);
            if (me == null)
                return RedirectToAction("Login", "Account");

            var cart = await _cart.GetAsync(HttpContext);
            if (!cart.Any())
                return RedirectToAction("Index", "Cart");

            decimal total = cart.Sum(x => x.Price * x.Qty);

            var orderItems = cart.Select(x => new OrderItem
            {
                ProductId = x.ProductId,
                Quantity = x.Qty,
                UnitPrice = x.Price
            }).ToList();

            var order = await _order.CreateOrderAsync(me.Id, orderItems, total, addressId);

            await _order.CompletePaymentAsync(order.Id, me.Id, total, method);
            await _cart.ClearAsync(HttpContext);

            TempData["msg"] = " Your order has been placed successfully!";
            return RedirectToAction("History", "Order");
        }
    }
}
