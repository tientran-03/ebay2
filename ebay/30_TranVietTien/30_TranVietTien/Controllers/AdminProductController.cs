using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _30_TranVietTien.Models;

public class AdminProductController : Controller
{
    private readonly CloneEbayDbContext _context;

    public AdminProductController(CloneEbayDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _context.Products.ToListAsync();
        return View(list);
    }

    [HttpPost]
    public IActionResult Hide(int id)
    {
        var p = _context.Products.Find(id);
        if (p == null) return NotFound();

        p.IsAuction = false;
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var p = _context.Products.Find(id);
        if (p == null) return NotFound();

        _context.Products.Remove(p);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
}
