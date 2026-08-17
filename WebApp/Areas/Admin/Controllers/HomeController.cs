using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Services;

namespace WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly DashboardApiService _dashboard;

        public HomeController(DashboardApiService dashboard) => _dashboard = dashboard;

        public async Task<IActionResult> Index()
        {
            var stats = await _dashboard.GetStats();
            return View(stats);
        }
    }
}
