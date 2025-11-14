using Microsoft.AspNetCore.Mvc;
using _30_TranVietTien.Models;
using Microsoft.EntityFrameworkCore;
using _30_TranVietTien.Services;

namespace _30_TranVietTien.Controllers
{
    public class SellerOrderController : Controller
    {
        private readonly CloneEbayDbContext _context;
        private readonly IUserService _userService;

        public SellerOrderController(CloneEbayDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }
        public async Task<IActionResult> Index()
        {
            var me = await _userService.GetCurrentAsync(HttpContext);
            if (me == null) return RedirectToAction("Login", "Account");

            var orders = await _context.OrderTables
                .Include(o => o.Buyer)
                .Include(o => o.Address)         
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Payments)
                .Include(o => o.ShippingInfos)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }


        [HttpPost]
        public async Task<IActionResult> Confirm(int id)
        {
            var order = await _context.OrderTables.FindAsync(id);
            if (order == null) return Json(new { success = false, message = "Không tìm thấy đơn hàng" });

            order.Status = "Đã xác nhận";
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var order = await _context.OrderTables.FindAsync(id);
            if (order == null) return Json(new { success = false });

            order.Status = status;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> PrintInvoice(int id)
        {
            var order = await _context.OrderTables
                .Include(o => o.Buyer)
                .Include(o => o.Address)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            return View("Invoice", order);
        }
    }
}
