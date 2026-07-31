using Microsoft.AspNetCore.Mvc;
using RequestForm.Interfaces;

namespace RequestForm.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _dashboardService.GetDashboard();

            return View(model);
        }
    }
}