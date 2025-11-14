using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _30_TranVietTien.Models;

namespace _30_TranVietTien.Controllers
{

    public class SellerDisputeController : Controller
    {
        private readonly CloneEbayDbContext _context;

        public SellerDisputeController(CloneEbayDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Details(int id)
        {
            var sellerId = HttpContext.Session.GetInt32("UserId");
            if (sellerId == null)
                return RedirectToAction("Login", "Account");

            var dispute = await _context.Disputes
                .Include(d => d.Order)
                    .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                .Include(d => d.RaisedByNavigation)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dispute == null)
                return NotFound();

            if (dispute.Order == null)
                return Content("❌ Order not found for this dispute.");

            // seller must own at least 1 product in this order
            bool isOwner = dispute.Order.OrderItems
                .Any(x => x.Product.SellerId == sellerId);

            if (!isOwner)
                return Unauthorized();

            return View(dispute);
        }


        [HttpPost]
        public IActionResult Handle(int disputeId, string action, string response)
        {
            var dispute = _context.Disputes.Find(disputeId);

            if (action == "approve")
                dispute.Status = "Approved";
            else if (action == "reject")
                dispute.Status = "Rejected";

            dispute.Resolution = response;

            _context.SaveChanges();

            TempData["Message"] = "Dispute updated successfully!";
            return RedirectToAction("Details", new { id = disputeId });
        }
        public async Task<IActionResult> Index()
        {
            var sellerId = HttpContext.Session.GetInt32("UserId");
            if (sellerId == null)
                return RedirectToAction("Login", "Account");
            var disputes = await _context.Disputes
                .Include(d => d.Order)
                    .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                .Include(d => d.RaisedByNavigation)
                .Where(d => d.Order.OrderItems
                    .Any(oi => oi.Product.SellerId == sellerId)) 
                .ToListAsync();

            return View(disputes);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int disputeId, string status)
        {
            var dispute = await _context.Disputes.FindAsync(disputeId);

            if (dispute == null)
                return NotFound();

            dispute.Status = status;
            _context.SaveChanges();

            TempData["Message"] = $"Dispute #{disputeId} updated to '{status}'";

            return RedirectToAction("Index");
        }

    }

}
