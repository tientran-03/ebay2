using _30_TranVietTien.Models;
using _30_TranVietTien.Services;
using Microsoft.AspNetCore.Mvc;

namespace _30_TranVietTien.Controllers
{
    public class AddressController : Controller
    {
        private readonly IAddressService _svc;
        private readonly IUserService _user;
        public AddressController(IAddressService s, IUserService u) { _svc = s; _user = u; }

        public async Task<IActionResult> Index()
        {
            var me = await _user.GetCurrentAsync(HttpContext);
            if (me == null) return RedirectToAction("Login", "Account");
            var list = await _svc.GetByUserAsync(me.Id);
            return View(list);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Address a, bool setDefault = false)
        {
            var me = await _user.GetCurrentAsync(HttpContext);
            if (me == null) return RedirectToAction("Login", "Account");
            await _svc.AddAsync(me.Id, a, setDefault);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> SetDefault(int id)
        {
            var me = await _user.GetCurrentAsync(HttpContext);
            if (me == null) return RedirectToAction("Login", "Account");
            await _svc.SetDefaultAsync(me.Id, id);
            return RedirectToAction(nameof(Index));
        }
    }

}
