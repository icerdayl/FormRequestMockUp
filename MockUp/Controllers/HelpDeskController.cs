using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RequestForm.Data;
using RequestForm.Interfaces;
using RequestForm.Models;
using RequestForm.Models.ViewModels;

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

        // Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var requests = await _context.Requests
                .Include(r => r.Status)
                .Include(r => r.RequestType)
                .ToListAsync();

            var model = new DashboardViewModel
            {
                TotalRequests = requests.Count,
                PendingRequests = requests.Count(r => r.Status!.StatusName == "Pending"),
                ApprovedRequests = requests.Count(r => r.Status!.StatusName == "Approved"),
                RejectedRequests = requests.Count(r => r.Status!.StatusName == "Rejected"),
                InProgressRequests = requests.Count(r => r.Status!.StatusName == "In Progress"),
                CompletedRequests = requests.Count(r => r.Status!.StatusName == "Completed"),

                RecentRequests = requests
                    .OrderByDescending(r => r.DateSubmitted)
                    .Take(5)
                    .ToList(),

                HighPriorityRequests = requests
                    .Where(r => r.Priority == "High")
                    .OrderByDescending(r => r.DateSubmitted)
                    .ToList()
            };

            return View(model);
        }

        // Request List
        public async Task<IActionResult> RequestList(string search, string status)
        {
            var query = _context.Requests
                .Include(r => r.Status)
                .Include(r => r.RequestType)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(r =>
                    r.Title.Contains(search) ||
                    r.ReferenceNumber.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(status) &&
                status != "All")
            {
                query = query.Where(r =>
                    r.Status.StatusName == status);
            }

            var requests = await query
                .OrderBy(r =>
                    r.Status.StatusName == "Pending" ? 0 : 1)
                .ThenByDescending(r => r.DateSubmitted)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.Status = status;

            return View(requests);
        }

        // Assignment
        public async Task<IActionResult> Assignment(string search, string status)
        {
            var approvedQuery = _context.Requests
                .Include(r => r.Status)
                .Include(r => r.RequestType)
                .Include(r => r.RequestAssignments)
                .Where(r =>
                    r.Status!.StatusName == "Approved" &&
                    !r.RequestAssignments.Any(a => a.IsCurrent));

            var assignedQuery = _context.Requests
                .Include(r => r.Status)
                .Include(r => r.RequestType)
                .Include(r => r.RequestAssignments)
                .Where(r =>
                    r.RequestAssignments.Any(a => a.IsCurrent));

            if (!string.IsNullOrWhiteSpace(search))
            {
                approvedQuery = approvedQuery.Where(r =>
                    r.Title.Contains(search) ||
                    r.ReferenceNumber.Contains(search));

                assignedQuery = assignedQuery.Where(r =>
                    r.Title.Contains(search) ||
                    r.ReferenceNumber.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(status) &&
                status != "All")
            {
                approvedQuery = approvedQuery.Where(r =>
                    r.Status!.StatusName == status);

                assignedQuery = assignedQuery.Where(r =>
                    r.Status!.StatusName == status);
            }

            var vm = new AssignmentPageViewModel
            {
                ApprovedRequests = await approvedQuery.ToListAsync(),
                AssignedRequests = await assignedQuery.ToListAsync()
            };

            ViewBag.Search = search;
            ViewBag.Status = status;

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignDeveloper(AssignmentViewModel model)
        {
            var previousAssignments = _context.RequestAssignments
                .Where(a => a.RequestId == model.RequestId && a.IsCurrent);

            foreach (var item in previousAssignments)
            {
                item.IsCurrent = false;
            }

            _context.RequestAssignments.Add(new RequestAssignment
            {
                RequestId = model.RequestId,
                AssignedTo = model.AssignedTo,
                AssignedBy = "Help Desk",
                AssignedDate = DateTime.Now,
                IsCurrent = true
            });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Assignment));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var request = await _context.Requests.FindAsync(id);

            if (request == null)
                return NotFound();

            request.StatusId = 2;   // Approved

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(RequestList));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var request = await _context.Requests.FindAsync(id);

            if (request == null)
                return NotFound();

            request.StatusId = 3;   // Rejected

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(RequestList));
        }

        public async Task<IActionResult> Review(int id)
        {
            var request = await _context.Requests
                .Include(r => r.RequestType)
                .Include(r => r.Status)
                .FirstOrDefaultAsync(r => r.RequestId == id);

            if (request == null)
                return NotFound();

            ViewBag.Approval = await _context.RequestApprovals
                .Where(a => a.RequestId == id)
                .OrderByDescending(a => a.DecisionDate)
                .FirstOrDefaultAsync();

            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(
            int RequestId,
            string status,
            string Remarks)
        {
            var request = await _context.Requests
                .FirstOrDefaultAsync(r => r.RequestId == RequestId);

            if (request == null)
                return NotFound();

            var approval = await _context.RequestApprovals
                .FirstOrDefaultAsync(a => a.RequestId == RequestId);

            if (approval == null)
            {
                approval = new RequestApproval
                {
                    RequestId = RequestId,
                    ApprovedBy = "Help Desk"
                };

                _context.RequestApprovals.Add(approval);
            }

            approval.Decision = status;
            approval.Remarks = Remarks ?? "";
            approval.DecisionDate = DateTime.Now;

            if (status == "Approved")
                request.StatusId = 2;

            if (status == "Rejected")
            {
                request.StatusId = 3;

                var currentAssignment = await _context.RequestAssignments
                    .FirstOrDefaultAsync(a =>
                        a.RequestId == RequestId &&
                        a.IsCurrent);

                if (currentAssignment != null)
                {
                    currentAssignment.IsCurrent = false;
                }

            }

                await _context.SaveChangesAsync();

            return RedirectToAction(nameof(RequestList));
        }

    }
}