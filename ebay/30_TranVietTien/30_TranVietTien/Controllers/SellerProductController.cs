using Microsoft.AspNetCore.Mvc;
using _30_TranVietTien.Models;
using _30_TranVietTien.Services;
using Microsoft.EntityFrameworkCore;

namespace _30_TranVietTien.Controllers
{
    public class SellerProductController : Controller
    {
        private readonly IProductService _service;
        private readonly IUserService _userService;
        private readonly CloneEbayDbContext _context;

        public SellerProductController(IProductService s, IUserService u, CloneEbayDbContext context)
        {
            _service = s;
            _userService = u;
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var me = await _userService.GetCurrentAsync(HttpContext);
            if (me == null) return RedirectToAction("Login", "Account");

            // Get statistics
            var productCount = await _context.Products.CountAsync(p => p.SellerId == me.Id);
            var orderCount = await _context.OrderTables
                .Include(o => o.OrderItems)
                .Where(o => o.OrderItems.Any(oi => oi.Product.SellerId == me.Id))
                .CountAsync();
            
            var disputeCount = await _context.Disputes
                .Where(d => d.Order.OrderItems.Any(oi => oi.Product.SellerId == me.Id) && d.Status == "Pending")
                .CountAsync();

            var revenue = await _context.OrderTables
                .Include(o => o.OrderItems)
                .Where(o => o.OrderItems.Any(oi => oi.Product.SellerId == me.Id) && o.Status == "Delivered")
                .SelectMany(o => o.OrderItems)
                .Where(oi => oi.Product.SellerId == me.Id)
                .SumAsync(oi => oi.Quantity * oi.UnitPrice);

            // Get recent orders
            var recentOrders = await _context.OrderTables
                .Include(o => o.Buyer)
                .Include(o => o.OrderItems)
                .Where(o => o.OrderItems.Any(oi => oi.Product.SellerId == me.Id))
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .ToListAsync();

            // Get low stock products
            var lowStockProducts = await _context.Products
                .Include(p => p.Inventories)
                .Where(p => p.SellerId == me.Id && p.Inventories.Any(i => i.Quantity <= 5))
                .Select(p => new { 
                    Title = p.Title, 
                    Stock = p.Inventories.FirstOrDefault().Quantity 
                })
                .ToListAsync();

            ViewBag.ProductCount = productCount;
            ViewBag.OrderCount = orderCount;
            ViewBag.DisputeCount = disputeCount;
            ViewBag.Revenue = revenue;
            ViewBag.RecentOrders = recentOrders;
            ViewBag.LowStockProducts = lowStockProducts;

            return View();
        }

        public async Task<IActionResult> Index()
        {
            var me = await _userService.GetCurrentAsync(HttpContext);
            if (me == null) return RedirectToAction("Login", "Account");

            var list = await _service.GetSellerProductsAsync(me.Id);
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Inventories)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return Json(new { success = false });

            var inv = product.Inventories?.FirstOrDefault();

            return Json(new
            {
                success = true,
                id = product.Id,
                title = product.Title,
                description = product.Description,
                price = product.Price,
                categoryId = product.CategoryId,
                image = product.Images,
                quantity = inv?.Quantity ?? 0
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product p, IFormFile? image, int Quantity)
        {
            var me = await _userService.GetCurrentAsync(HttpContext);
            if (me == null) return Json(new { success = false, message = "Chưa đăng nhập" });

            p.SellerId = me.Id;
            await _service.CreateAsync(p, image);

            var inv = new Inventory
            {
                ProductId = p.Id,
                Quantity = Quantity,
                LastUpdated = DateTime.Now
            };
            _context.Inventories.Add(inv);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product p, IFormFile? image, int Quantity)
        {
            if (!ModelState.IsValid) return Json(new { success = false });

            await _service.UpdateAsync(p, image);

            var inv = await _context.Inventories.FirstOrDefaultAsync(i => i.ProductId == p.Id);
            if (inv == null)
            {
                inv = new Inventory
                {
                    ProductId = p.Id,
                    Quantity = Quantity,
                    LastUpdated = DateTime.Now
                };
                _context.Inventories.Add(inv);
            }
            else
            {
                inv.Quantity = Quantity;
                inv.LastUpdated = DateTime.Now;
                _context.Inventories.Update(inv);
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var inv = await _context.Inventories.FirstOrDefaultAsync(i => i.ProductId == id);
                if (inv != null) _context.Inventories.Remove(inv);

                await _service.DeleteAsync(id);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleHide(int id, bool hide)
        {
            await _service.HideAsync(id, hide);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> AddCoupon(Coupon coupon)
        {
            if (!ModelState.IsValid) return Json(new { success = false });

            var me = await _userService.GetCurrentAsync(HttpContext);
            if (me == null) return Json(new { success = false, message = "Chưa đăng nhập" });

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == coupon.ProductId && p.SellerId == me.Id);
            if (product == null) return Json(new { success = false, message = "Không tìm thấy sản phẩm hoặc không có quyền thêm mã cho sản phẩm này." });

            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var me = await _userService.GetCurrentAsync(HttpContext);
            if (me == null) return Json(new List<object>());

            var products = await _context.Products
                .Where(p => p.SellerId == me.Id)
                .Select(p => new { id = p.Id, title = p.Title })
                .ToListAsync();

            return Json(products);
        }
        [HttpGet]
        public async Task<IActionResult> GetBuyerByProduct(int productId)
        {
            var sellerId = HttpContext.Session.GetInt32("UserId");
            if (sellerId == null)
                return Json(new { success = false });

            var buyer = await (from o in _context.OrderTables
                               join oi in _context.OrderItems on o.Id equals oi.OrderId
                               where oi.ProductId == productId
                               select o.BuyerId)
                               .FirstOrDefaultAsync();

            if (buyer == null)
                return Json(new { success = false });

            return Json(new { success = true, buyerId = buyer });
        }

    }
}
