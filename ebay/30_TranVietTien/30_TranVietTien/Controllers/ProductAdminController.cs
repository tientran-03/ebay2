using Microsoft.AspNetCore.Mvc;
using _30_TranVietTien.Models;
using Microsoft.EntityFrameworkCore;


namespace _30_TranVietTien.Controllers
{
    public class ProductAdminController : Controller
    {
        private readonly CloneEbayDbContext _context;
        public ProductAdminController(CloneEbayDbContext context) => _context = context;


        public IActionResult Index() => View(_context.Products.Include(p => p.Seller).ToList());


        public IActionResult Hide(int id)
        {
            var p = _context.Products.Find(id);
            if (p == null) return NotFound();


            p.IsAuction = false;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        public IActionResult Delete(int id)
        {
            var p = _context.Products.Find(id);
            if (p == null) return NotFound();


            _context.Products.Remove(p);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}