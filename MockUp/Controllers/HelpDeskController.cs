using Microsoft.AspNetCore.Mvc;
using RequestForm.Interfaces;
using RequestForm.Models;
using RequestForm.Models.ViewModels;
using RequestForm.Data;
using Microsoft.EntityFrameworkCore;

namespace RequestForm.Controllers
{
    public class HelpDeskController : Controller
    {
        private readonly IRequestService _requestService;
        private readonly ApplicationDbContext _context;

        public HelpDeskController(
            IRequestService requestService,
            ApplicationDbContext context)
        {
            _requestService = requestService;
            _context = context;
        }


        public async Task<IActionResult> RequestList()
        {
            var requests = await _requestService.GetAll();

            return View(requests);
        }

        public IActionResult ApprovalList()
        {
            return View();
        }

        public async Task<IActionResult> Assignment()
        {
            var requests = await _requestService.GetAll();

            return View(requests);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignDeveloper(AssignmentViewModel model)
        {
            var assignment = new RequestAssignment
            {
                RequestId = model.RequestId,
                AssignedTo = model.AssignedTo,
                AssignedBy = "Help Desk",
                AssignedDate = DateTime.Now,
                IsCurrent = true
            };

            _context.RequestAssignments.Add(assignment);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Assignment));
        }

        public async Task<IActionResult> Dashboard()
        {
            var total = await _context.Requests.CountAsync();

            ViewBag.TotalRequests = total;

            return View();
        }
    }
}