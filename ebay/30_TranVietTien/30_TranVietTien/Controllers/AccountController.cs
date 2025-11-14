using _30_TranVietTien.Models;
using _30_TranVietTien.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _30_TranVietTien.Controllers
{
    public class AccountController : Controller
    {
        private readonly CloneEbayDbContext _context;
        private readonly IAuthService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(CloneEbayDbContext context, IAuthService authService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Please enter your email and password.";
                return View();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                ViewBag.Error = "Account not found.";
                return View();
            }

            if (!_authService.VerifyPassword(password, user.Password!))
            {
                ViewBag.Error = "Incorrect password.";
                return View();
            }

            HttpContext.Session.SetString("Username", user.Username ?? "User");
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Role", user.Role ?? "Buyer");

            if (user.Role == "Seller")
            {
                return RedirectToAction("Index", "SellerProduct");
            }
            else if (user.Role == "Admin")
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(string username, string email, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Please fill in all fields.";
                return View();
            }
            if (await _context.Users.AnyAsync(u => u.Email == email))
            {
                ViewBag.Error = "Email already registered.";
                return View();
            }

            var user = new User
            {
                Username = username,
                Email = email,
                Password = _authService.HashPassword(password),
                Role = "Buyer"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetInt32("UserId", user.Id);

            return RedirectToAction("Index", "Home");
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
