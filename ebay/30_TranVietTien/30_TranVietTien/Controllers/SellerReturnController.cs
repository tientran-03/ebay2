using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _30_TranVietTien.Models;

namespace _30_TranVietTien.Controllers
{
    public class SellerReturnController : Controller
    {
        private readonly CloneEbayDbContext _context;

        public SellerReturnController(CloneEbayDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var requests = await _context.ReturnRequests
                .Include(r => r.Order)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(requests);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status, string response)
        {
            var req = await _context.ReturnRequests.FindAsync(id);
            if (req == null) return Json(new { success = false });

            req.Status = status;
            req.SellerResponse = response;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

    }
}
