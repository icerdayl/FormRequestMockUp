using Microsoft.AspNetCore.Mvc;

namespace MockUp.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}