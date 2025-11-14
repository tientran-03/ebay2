using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _30_TranVietTien.Models;

namespace _30_TranVietTien.Controllers
{

    public class DisputeController : Controller
    {
        private readonly CloneEbayDbContext _context;
        private readonly IHttpContextAccessor _http;

        public DisputeController(CloneEbayDbContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            _http = accessor;
        }

        [HttpPost]
        public IActionResult Create(int orderId, string reason, string description)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            var dispute = new Dispute
            {
                OrderId = orderId,
                Description = $"{reason}: {description}",
                RaisedBy = userId,
                Status = "Pending"
            };

            _context.Disputes.Add(dispute);
            _context.SaveChanges();

            TempData["Message"] = "Your request has been sent to the seller!";
            return RedirectToAction("History", "Order");
        }

        public IActionResult Report(int orderId)
        {
            var order = _context.OrderTables
                .Include(o => o.OrderItems).ThenInclude(i => i.Product)
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null) return NotFound();

            return View(order);
        }
    }

}
