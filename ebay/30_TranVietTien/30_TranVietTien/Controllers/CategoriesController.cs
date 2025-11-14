using _30_TranVietTien.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _30_TranVietTien.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly CloneEbayDbContext _context;

        public CategoriesController(CloneEbayDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Json(new { success = false, message = "Name required" });

            var category = new Category { Name = name };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, string name)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return Json(new { success = false });

            category.Name = name;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return Json(new { success = false });

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}
