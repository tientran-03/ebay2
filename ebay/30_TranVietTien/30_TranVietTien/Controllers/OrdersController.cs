// Controllers/OrderController.cs
using _30_TranVietTien.Models;
using _30_TranVietTien.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _30_TranVietTien.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly CloneEbayDbContext _ctx;

        public OrderController(IOrderService orderService, CloneEbayDbContext ctx)
        {
            _orderService = orderService;
            _ctx = ctx;
        }
        [HttpPost]
        public async Task<IActionResult> Checkout(string paymentMethod, int addressId, decimal finalTotal)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemVM>>("Cart") ?? new();
            if (!cart.Any()) return RedirectToAction("Index", "Cart");

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var validAddress = await _ctx.Addresses
                .AnyAsync(a => a.Id == addressId && a.UserId == userId.Value);

            if (!validAddress)
            {
                TempData["msg"] = "Invalid address";
                return RedirectToAction("Index", "Cart");
            }

            var items = cart.Select(x => new OrderItem
            {
                ProductId = x.ProductId,
                Quantity = x.Qty,
                UnitPrice = x.Price
            }).ToList();
            var total = finalTotal;

            var order = await _orderService.CreateOrderAsync(userId.Value, items, total, addressId);
            var paid = await _orderService.CompletePaymentAsync(order.Id, userId.Value, total, paymentMethod);

            if (paid) HttpContext.Session.Remove("Cart");
            TempData["msg"] = paid ? "Order placed successfully." : "Payment failed.";

            return RedirectToAction("History");
        }



        public async Task<IActionResult> History()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");
            var orders = await _orderService.GetUserOrdersAsync(userId.Value);
            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

    }
}
