using _30_TranVietTien.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _30_TranVietTien.Controllers
{
    public class HomeController : Controller
    {
        private readonly CloneEbayDbContext _context;

        public HomeController(CloneEbayDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string q, int? cat, int page = 1)
        {
            int pageSize = 6;

            ViewBag.Categories = await _context.Categories.ToListAsync();

            var productsQuery = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(q))
                productsQuery = productsQuery.Where(p => p.Title.Contains(q) || p.Category.Name.Contains(q));

            if (cat.HasValue && cat.Value > 0)
                productsQuery = productsQuery.Where(p => p.CategoryId == cat.Value);

            int totalItems = await productsQuery.CountAsync();

            var products = await productsQuery
                .OrderByDescending(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentQuery = q;
            ViewBag.SelectedCat = cat;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(products);
        }

    }
}
