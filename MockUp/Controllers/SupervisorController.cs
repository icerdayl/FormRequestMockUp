using Microsoft.AspNetCore.Mvc;
using RequestForm.Data;
using RequestForm.Interfaces;
using RequestForm.Services;

namespace RequestForm.Controllers
{
    public class SupervisorController : Controller
    {
        private readonly ISupervisorService _supervisorService;
        private readonly ApplicationDbContext _context;

        public SupervisorController(
            ISupervisorService supervisorService,
            ApplicationDbContext context)
        {
            _supervisorService = supervisorService;
            _context = context;
        }

        // APPROVAL LIST
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

        // REVIEW REQUEST
        public async Task<IActionResult> Review(int id)
        {
            var request =
                await _supervisorService.GetRequestForReview(id);

            if (request == null)
                return NotFound();

            ViewBag.Remarks = await _context.GetApprovalRemarksAsync(id);

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