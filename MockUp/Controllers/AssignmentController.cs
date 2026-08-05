using Microsoft.AspNetCore.Mvc;

namespace RequestForm.Controllers
{
    public class AssignmentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}