using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _30_TranVietTien.Models;

public class AdminController : Controller
{
    private readonly CloneEbayDbContext _context;

    public AdminController(CloneEbayDbContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Dashboard()
    {
        ViewBag.TotalUsers = await _context.Users.CountAsync();
        ViewBag.TotalProducts = await _context.Products.CountAsync();
        ViewBag.TotalOrders = await _context.OrderTables.CountAsync();
        ViewBag.TotalRevenue = await _context.Payments
            .Where(x => x.Status == "Paid")
            .SumAsync(x => (decimal?)x.Amount) ?? 0;

        return View();
    }
}
