using Microsoft.AspNetCore.Mvc;
using RequestForm.Data;
using RequestForm.Interfaces;
using RequestForm.Services;

namespace RequestForm.Controllers
{
    public class ManagerController : Controller
    {
        private readonly IManagerService _managerService;
        private readonly ApplicationDbContext _context;

        public ManagerController(
            IManagerService managerService,
            ApplicationDbContext context)
        {
            _managerService = managerService;
            _context = context;
        }

        // APPROVAL LIST
        public async Task<IActionResult> Index(string? search)
        {
            var requests =
                await _managerService.GetPendingApprovals(search);

            ViewBag.Search = search;

            return View(requests);
        }

        // REVIEW REQUEST
        public async Task<IActionResult> Review(int id)
        {
            var request =
                await _managerService.GetRequestForReview(id);

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
                await _managerService.ProcessApproval(
                    RequestId,
                    decision,
                    remarks);

            if (request == null)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}