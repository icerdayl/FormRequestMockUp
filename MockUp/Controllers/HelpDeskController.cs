using Microsoft.AspNetCore.Mvc;
using RequestForm.Interfaces;
using RequestForm.Models.ViewModels;

namespace RequestForm.Controllers
{
    public class HelpDeskController : Controller
    {
        private readonly IHelpDeskService _helpDeskService;

        public HelpDeskController(
            IHelpDeskService helpDeskService)
        {
            _helpDeskService = helpDeskService;
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

            ViewBag.Approval =
                await _helpDeskService.GetLatestApproval(id);

            return View(request);
        }

        // Approve / Reject
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(
            int RequestId,
            string status,
            string Remarks)
        {
            var success =
                await _helpDeskService.UpdateStatus(
                    RequestId,
                    status,
                    Remarks);

            if (!success)
                return NotFound();

            return RedirectToAction(nameof(RequestList));
        }
    }
}