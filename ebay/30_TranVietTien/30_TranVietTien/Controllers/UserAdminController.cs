using Microsoft.AspNetCore.Mvc;
using _30_TranVietTien.Models;
using Microsoft.EntityFrameworkCore;


namespace _30_TranVietTien.Controllers
{
    public class UserAdminController : Controller
    {
        private readonly CloneEbayDbContext _context;
        public UserAdminController(CloneEbayDbContext context) => _context = context;


        public IActionResult Index()
        {
            var users = _context.Users.ToList();
            return View(users);
        }


        public IActionResult ToggleLock(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();


            user.Role = user.Role == "Locked" ? "Buyer" : "Locked";
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}