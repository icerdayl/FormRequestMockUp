using Microsoft.AspNetCore.Mvc;

namespace ChangeRequest.Controllers
{
    public class ApprovalController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}