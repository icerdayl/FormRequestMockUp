using Microsoft.AspNetCore.Mvc;

namespace MockUp.Controllers
{
    public class ApprovalController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}