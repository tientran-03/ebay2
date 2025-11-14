using Microsoft.AspNetCore.Mvc;
using _30_TranVietTien.Services;

namespace _30_TranVietTien.Controllers
{
    public class ReportController : Controller
    {
        private readonly IReportService _service;
        private readonly IUserService _userService;

        public ReportController(IReportService service, IUserService userService)
        {
            _service = service;
            _userService = userService;
        }

        public async Task<IActionResult> Weekly()
        {
            var me = await _userService.GetCurrentAsync(HttpContext);
            if (me == null) return RedirectToAction("Login", "Account");
            var data = await _service.GetWeeklyReportAsync(me.Id);
            return View(data);
        }

        public async Task<IActionResult> Monthly()
        {
            var me = await _userService.GetCurrentAsync(HttpContext);
            if (me == null) return RedirectToAction("Login", "Account");
            var data = await _service.GetMonthlyReportAsync(me.Id);
            return View(data);
        }
    }
}
