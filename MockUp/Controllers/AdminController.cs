using Microsoft.AspNetCore.Mvc;

namespace RequestForm.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}