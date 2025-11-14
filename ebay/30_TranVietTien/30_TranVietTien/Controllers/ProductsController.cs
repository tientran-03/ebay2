using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _30_TranVietTien.Models;
using _30_TranVietTien.Services;

namespace _30_TranVietTien.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly CloneEbayDbContext _context;

        public ProductsController(IProductService productService, CloneEbayDbContext context)
        {
            _productService = productService;
            _context = context;
        }
        public async Task<IActionResult> Index(int? categoryId, string? q, decimal? min, decimal? max, string? sort, int page = 1)
        {
            var filter = new ProductFilter
            {
                CategoryId = categoryId,
                Q = q,
                Min = min,
                Max = max,
                Sort = sort ?? "newest",
                Page = page,
                PageSize = 12
            };

            var pagedProducts = await _productService.GetPagedAsync(filter);
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Filter = filter;

            return View(pagedProducts);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetDetailAsync(id);
            if (product == null) return NotFound();
            int? userId = HttpContext.Session.GetInt32("UserId");
            bool hasPurchased = false;

            if (userId.HasValue)
            {
                hasPurchased = await _context.OrderTables
                    .Include(o => o.OrderItems)
                    .AnyAsync(o =>
                        o.BuyerId == userId.Value &&
                        o.Status == "Delivered" &&
                        o.OrderItems.Any(i => i.ProductId == product.Id)
                    );
            }

            ViewBag.HasPurchased = hasPurchased;
            return View(product);
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product model, IFormFile? image)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Categories = _context.Categories.ToList();
                    return View(model);
                }
                int? sellerId = HttpContext.Session.GetInt32("UserId");
                if (sellerId == null)
                {
                    TempData["Error"] = "Vui lòng đăng nhập để thêm sản phẩm.";
                    return RedirectToAction("Login", "Account");
                }

                model.SellerId = sellerId.Value;
                await _productService.CreateAsync(model, image);

                TempData["Success"] = "Thêm sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tạo sản phẩm: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();

            ViewBag.Categories = _context.Categories.ToList();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product model, IFormFile? image)
        {
            try
            {
                await _productService.UpdateAsync(model, image);
                TempData["Success"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi cập nhật: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _productService.DeleteAsync(id);
                TempData["Success"] = "Đã xóa sản phẩm!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể xóa: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        public async Task<IActionResult> ToggleHide(int id, bool hide)
        {
            await _productService.HideAsync(id, hide);
            return RedirectToAction(nameof(Index));
        }
    }
}
