using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _30_TranVietTien.Models;

public class AdminUserController : Controller
{
    private readonly CloneEbayDbContext _context;

    public AdminUserController(CloneEbayDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _context.Users.ToListAsync();
        return View(users);
    }

    [HttpPost]
    public IActionResult ToggleLock(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null) return NotFound();

        user.Role = user.Role == "Locked" ? "User" : "Locked";

        _context.SaveChanges();
        return RedirectToAction("Index");
    }
}
