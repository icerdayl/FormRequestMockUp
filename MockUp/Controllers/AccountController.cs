using Microsoft.AspNetCore.Mvc;

namespace RequestForm.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}