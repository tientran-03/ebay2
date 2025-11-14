using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _30_TranVietTien.Models;

namespace _30_TranVietTien.Controllers
{
    public class ReturnRequestController : Controller
    {
        private readonly CloneEbayDbContext _context;

        public ReturnRequestController(CloneEbayDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var requests = await _context.ReturnRequests
                .Include(r => r.Order)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(requests);
        }
        [HttpGet]
        public async Task<IActionResult> Create(int orderId)
        {
            var order = await _context.OrderTables
                .Include(o => o.OrderItems)
                   .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return NotFound();

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Create(int orderId, string reason, IFormFile? evidenceFile)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            string imagePath = null;

            if (evidenceFile != null && evidenceFile.Length > 0)
            {
                var folder = Path.Combine("wwwroot", "images", "returns");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid() + Path.GetExtension(evidenceFile.FileName);
                var path = Path.Combine(folder, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await evidenceFile.CopyToAsync(stream);
                }

                imagePath = "/images/returns/" + fileName;
            }

            var request = new ReturnRequest
            {
                OrderId = orderId,
                UserId = userId.Value,
                Reason = reason,
                EvidenceImage = imagePath,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            _context.ReturnRequests.Add(request);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


    }
}
