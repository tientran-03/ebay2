using Microsoft.AspNetCore.Mvc;
using _30_TranVietTien.Models;
using Microsoft.EntityFrameworkCore;


namespace _30_TranVietTien.Controllers
{
    public class OrderAdminController : Controller
    {
        private readonly CloneEbayDbContext _context;
        public OrderAdminController(CloneEbayDbContext context) => _context = context;


        public IActionResult Index()
        {
            var orders = _context.OrderTables
            .Include(o => o.Buyer)
            .Include(o => o.Address)
            .ToList();
            return View(orders);
        }
    }
}