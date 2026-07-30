using Microsoft.AspNetCore.Mvc;

namespace RequestForm.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}