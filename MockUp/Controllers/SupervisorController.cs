using Microsoft.AspNetCore.Mvc;
using RequestForm.Interfaces;

namespace RequestForm.Controllers
{
    public class SupervisorController : Controller
    {
        private readonly ISupervisorService _supervisorService;

        public SupervisorController(
            ISupervisorService supervisorService)
        {
            _supervisorService = supervisorService;
        }

        public async Task<IActionResult> Index(
            string? search)
        {
            var requests =
                await _supervisorService.GetPendingApprovals(
                    search,
                    null);

            ViewBag.Search = search;

            return View(requests);
        }

        public async Task<IActionResult> Review(int id)
        {
            var request =
                await _supervisorService.GetRequestForReview(id);

            if (request == null)
                return NotFound();

            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(
            int RequestId,
            string decision,
            string remarks)
        {
            var request =
                await _supervisorService.ProcessApproval(
                    RequestId,
                    decision,
                    remarks);

            if (request == null)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }
    }
}