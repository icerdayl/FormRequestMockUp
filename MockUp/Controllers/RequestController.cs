using Microsoft.AspNetCore.Mvc;

namespace MockUp.Controllers
{
    public class RequestController : Controller
    {
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult MyRequests()
        {
            return View();
        }
        public IActionResult Details()
        {
            return View();
        }
    }
}