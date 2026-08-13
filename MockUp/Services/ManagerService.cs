using Microsoft.EntityFrameworkCore;
using RequestForm.Data;
using RequestForm.Interfaces;
using RequestForm.Models;

namespace RequestForm.Services
{
    public class ManagerService : IManagerService
    {
        private readonly ApplicationDbContext _context;

        public ManagerService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // GET PENDING MANAGER APPROVALS
        // ==========================================

        public async Task<List<Request>> GetPendingApprovals(
            string? search)
        {
            var query = _context.Requests
                .Include(r => r.RequestType)
                .Include(r => r.Status)
                .Where(r => r.Status!.StatusName == "Approved by Supervisor")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(r =>
                    r.Title.Contains(search) ||
                    r.ReferenceNumber.Contains(search));
            }

            return await query
                .OrderByDescending(r => r.DateSubmitted)
                .ToListAsync();
        }


        // ==========================================
        // GET REQUEST FOR REVIEW
        // ==========================================

        public async Task<Request?> GetRequestForReview(int id)
        {
            return await _context.Requests
                .Include(r => r.RequestType)
                .Include(r => r.Status)
                .FirstOrDefaultAsync(r =>
                    r.RequestId == id);
        }


        // ==========================================
        // PROCESS MANAGER APPROVAL
        // ==========================================

        public async Task<Request?> ProcessApproval(
            int requestId,
            string decision,
            string remarks)
        {
            var request = await _context.Requests
                .FirstOrDefaultAsync(r =>
                    r.RequestId == requestId);

            if (request == null)
                return null;

            if (request.StatusId != 3)
                return null;

            var approval = new RequestApproval
            {
                RequestId = requestId,
                ApprovedBy = "Dummy Manager",
                Decision = decision,
                Remarks = remarks ?? "",
                DecisionDate = DateTime.Now
            };

            _context.RequestApprovals.Add(approval);


            // ==========================================
            // APPROVED
            // ==========================================

            if (decision == "Approved")
            {
                request.StatusId = 4;
            }


            // ==========================================
            // REJECTED
            // ==========================================

            else if (decision == "Rejected")
            {
                request.StatusId = 5;
            }


            await _context.SaveChangesAsync();

            return request;
        }
    }
}