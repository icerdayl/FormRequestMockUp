using Microsoft.AspNetCore.Mvc;
using RequestForm.Data;
using RequestForm.Interfaces;
using RequestForm.Models.ViewModels;
using RequestForm.Services;

namespace RequestForm.Controllers
{
    public class HelpDeskController : Controller
    {
        private readonly IHelpDeskService _helpDeskService;
        private readonly ApplicationDbContext _context;

        public HelpDeskController(
            IHelpDeskService helpDeskService,
            ApplicationDbContext context)
        {
            _helpDeskService = helpDeskService;
            _context = context;
        }

        // Request List
        public async Task<IActionResult> RequestList(
            string? search,
            string? status)
        {
            var requests =
                await _helpDeskService.GetRequestList(
                    search,
                    status);

            ViewBag.Search = search;
            ViewBag.Status = status;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView(
                    "_RequestListTable",
                    requests);
            }

            return View(requests);
        }

        // Assignment
        public async Task<IActionResult> Assignment(
            string? search,
            string? status)
        {
            var model =
                await _helpDeskService.GetAssignments(
                    search,
                    status);

            ViewBag.Search = search;
            ViewBag.Status = status;

            return View(model);
        }

        // Assign Developer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignDeveloper(
            AssignmentViewModel model)
        {
            var success =
                await _helpDeskService.AssignDeveloper(model);

            if (!success)
                return NotFound();

            return RedirectToAction(nameof(Assignment));
        }

        // Review
        public async Task<IActionResult> Review(int id)
        {
            var request =
                await _helpDeskService.GetRequestForReview(id);

            if (request == null)
                return NotFound();

            ViewBag.Remarks = await _context.GetApprovalRemarksAsync(id);

            return View(request);
        }

        // Approve / Reject
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(
            int RequestId,
            string decision,
            string remarks)
        {
            var success =
                await _helpDeskService.UpdateStatus(
                    RequestId,
                    decision,
                    remarks);

            if (!success)
                return NotFound();

            return RedirectToAction(nameof(RequestList));
        }
    }
}