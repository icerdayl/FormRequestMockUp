using Microsoft.EntityFrameworkCore;
using RequestForm.Data;
using RequestForm.Interfaces;
using RequestForm.Models;
using RequestForm.Models.ViewModels;

namespace RequestForm.Services
{
    public class HelpDeskService : IHelpDeskService
    {
        private readonly ApplicationDbContext _context;

        public HelpDeskService(ApplicationDbContext context)
        {
            _context = context;
        }
                
        public async Task<List<Request>> GetRequestList(
            string? search,
            string? status)
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
                    r.Status!.StatusName == status);
            }

            return await query
                .OrderBy(r =>
                    r.Status!.StatusName == "Pending" ? 0 : 1)
                .ThenByDescending(r => r.DateSubmitted)
                .ToListAsync();
        }

        public async Task<AssignmentPageViewModel> GetAssignments(
            string? search,
            string? status)
        {
            var approvedQuery = _context.Requests
                .Include(r => r.Status)
                .Include(r => r.RequestType)
                .Include(r => r.RequestAssignments)
                .Where(r =>
                    r.Status!.StatusName == "Approved by Manager" &&
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

            return new AssignmentPageViewModel
            {
                ApprovedRequests =
                    await approvedQuery.ToListAsync(),

                AssignedRequests =
                    await assignedQuery.ToListAsync()
            };
        }

        public async Task<bool> AssignDeveloper(
            AssignmentViewModel model)
        {
            var request = await _context.Requests
                .FirstOrDefaultAsync(r =>
                    r.RequestId == model.RequestId);

            if (request == null)
                return false;

            var previousAssignments = await _context.RequestAssignments
                .Where(a =>
                    a.RequestId == model.RequestId &&
                    a.IsCurrent)
                .ToListAsync();

            foreach (var assignment in previousAssignments)
            {
                assignment.IsCurrent = false;
            }

            _context.RequestAssignments.Add(
                new RequestAssignment
                {
                    RequestId = model.RequestId,
                    AssignedTo = model.AssignedTo,
                    AssignedBy = "Help Desk",
                    AssignedDate = DateTime.Now,
                    IsCurrent = true
                });

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateStatus(
            int requestId,
            string status,
            string remarks)
        {
            var request = await _context.Requests
                .FirstOrDefaultAsync(r =>
                    r.RequestId == requestId);

            if (request == null)
                return false;

            var approval = await _context.RequestApprovals
                .FirstOrDefaultAsync(a =>
                    a.RequestId == requestId &&
                    a.ApprovedBy == "Help Desk");

            if (approval == null)
            {
                approval = new RequestApproval
                {
                    RequestId = requestId,
                    ApprovedBy = "Help Desk"
                };

                _context.RequestApprovals.Add(approval);
            }

            approval.Decision = status;
            approval.Remarks = remarks ?? "";
            approval.DecisionDate = DateTime.Now;

            if (status == "Approved")
            {
                request.StatusId = 2;
            }

            if (status == "Rejected")
            {
                request.StatusId = 5;

                var currentAssignment =
                    await _context.RequestAssignments
                        .FirstOrDefaultAsync(a =>
                            a.RequestId == requestId &&
                            a.IsCurrent);

                if (currentAssignment != null)
                {
                    currentAssignment.IsCurrent = false;
                }
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Request?> GetRequestForReview(int id)
        {
            return await _context.Requests
                .Include(r => r.RequestType)
                .Include(r => r.Status)
                .FirstOrDefaultAsync(r =>
                    r.RequestId == id);
        }

        public async Task<RequestApproval?> GetLatestApproval(
            int requestId)
        {
            return await _context.RequestApprovals
                .Where(a => a.RequestId == requestId)
                .OrderByDescending(a => a.DecisionDate)
                .FirstOrDefaultAsync();
        }
    }
}