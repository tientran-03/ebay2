using Microsoft.AspNetCore.Mvc;
using _30_TranVietTien.Models;
using Microsoft.EntityFrameworkCore;


namespace _30_TranVietTien.Controllers
{
    public class StatsAdminController : Controller
    {
        private readonly CloneEbayDbContext _context;
        public StatsAdminController(CloneEbayDbContext context) => _context = context;


        public IActionResult Index()
        {
            var today = DateTime.Today;
            var model = new StatsViewModel
            {
                RevenueToday = _context.Payments
            .Where(p => p.PaidAt.Value.Date == today)
            .Sum(p => (decimal?)p.Amount) ?? 0,


                RevenueThisMonth = _context.Payments
            .Where(p => p.PaidAt.Value.Month == today.Month)
            .Sum(p => (decimal?)p.Amount) ?? 0,


                NewUsersThisQuarter = _context.Users.Count() // placeholder
            };


            return View(model);
        }
    }
}