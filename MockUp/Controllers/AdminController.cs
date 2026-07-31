using Microsoft.AspNetCore.Mvc;

namespace ChangeRequest.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}