using _30_TranVietTien.Models;
using _30_TranVietTien.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _30_TranVietTien.Controllers
{
    public class CartController : Controller
    {
        private readonly CloneEbayDbContext _ctx;
        public CartController(CloneEbayDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                ViewBag.Addresses = await _ctx.Addresses
                    .Where(a => a.UserId == userId)
                    .ToListAsync();

                ViewBag.DefaultAddress = await _ctx.Addresses
                    .FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefault == true);
            }

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemVM>>("Cart") ?? new();
            return View(cart);
        }



        [HttpPost]
        public IActionResult Add(int id, string name, decimal price, string image)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemVM>>("Cart") ?? new List<CartItemVM>();
            var exist = cart.FirstOrDefault(x => x.ProductId == id);

            if (exist != null)
                exist.Qty += 1;
            else
                cart.Add(new CartItemVM { ProductId = id, Name = name, Price = price, Image = image, Qty = 1 });

            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Update(int productId, int quantity)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemVM>>("Cart") ?? new List<CartItemVM>();
            var item = cart.FirstOrDefault(x => x.ProductId == productId);
            if (item != null) item.Qty = quantity;
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Remove(int productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemVM>>("Cart") ?? new List<CartItemVM>();
            cart.RemoveAll(x => x.ProductId == productId);
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return RedirectToAction("Index");
        }
    }
}
