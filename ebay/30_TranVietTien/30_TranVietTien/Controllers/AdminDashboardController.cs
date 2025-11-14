using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _30_TranVietTien.Models;

public class AdminDashboardController : Controller
{
    private readonly CloneEbayDbContext _context;

    public AdminDashboardController(CloneEbayDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        DateTime today = DateTime.Today;
        DateTime monthStart = new DateTime(today.Year, today.Month, 1);

        int quarter = (today.Month - 1) / 3 + 1;
        DateTime quarterStart = new DateTime(today.Year, (quarter - 1) * 3 + 1, 1);

        var model = new DashboardStatsViewModel
        {
            // DOANH THU
            RevenueToday = _context.OrderTables
                .Where(o => o.OrderDate >= today)
                .Sum(o => (decimal?)o.TotalPrice ?? 0),

            RevenueMonth = _context.OrderTables
                .Where(o => o.OrderDate >= monthStart)
                .Sum(o => (decimal?)o.TotalPrice ?? 0),

            RevenueQuarter = _context.OrderTables
                .Where(o => o.OrderDate >= quarterStart)
                .Sum(o => (decimal?)o.TotalPrice ?? 0),
            OrdersToday = _context.OrderTables.Count(o => o.OrderDate >= today),
            OrdersMonth = _context.OrderTables.Count(o => o.OrderDate >= monthStart),
            OrdersQuarter = _context.OrderTables.Count(o => o.OrderDate >= quarterStart),
        };

        return View(model);
    }
}
