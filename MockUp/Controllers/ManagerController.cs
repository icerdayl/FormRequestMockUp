using Microsoft.AspNetCore.Mvc;
using RequestForm.Interfaces;

namespace RequestForm.Controllers
{
    public class ManagerController : Controller
    {
        private readonly IManagerService _managerService;

        public ManagerController(
            IManagerService managerService)
        {
            _managerService = managerService;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var requests =
                await _managerService.GetPendingApprovals(search);

            ViewBag.Search = search;

            return View(requests);
        }

        public async Task<IActionResult> Review(int id)
        {
            var request =
                await _managerService.GetRequestForReview(id);

            if (request == null)
                return NotFound();

            return View(request);
        }

        [HttpPost]
        [Route("Manager/UpdateStatus")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(
            int RequestId,
            string decision,
            string remarks)
        {
            Console.WriteLine("=================================");
            Console.WriteLine("MANAGER UPDATE STATUS CALLED");
            Console.WriteLine($"RequestId: {RequestId}");
            Console.WriteLine($"Decision: {decision}");
            Console.WriteLine($"Remarks: {remarks}");
            Console.WriteLine("=================================");

            var request =
                await _managerService.ProcessApproval(
                    RequestId,
                    decision,
                    remarks);

            if (request == null)
            {
                Console.WriteLine("ManagerService returned NULL.");
                return NotFound();
            }

            Console.WriteLine("Manager approval successful.");

            return RedirectToAction(nameof(Index));
        }
    }
}