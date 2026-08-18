using Microsoft.EntityFrameworkCore;
using RequestForm.Data;
using RequestForm.Interfaces;
using RequestForm.Models;

namespace RequestForm.Services
{
    public class SupervisorService : ISupervisorService
    {
        private readonly ApplicationDbContext _context;

        public SupervisorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Request>> GetPendingApprovals(
            string? search,
            string? status)
        {
            var query = _context.Requests
                .Include(r => r.Status)
                .Include(r => r.TicketType)
                .Where(r =>
                    r.Status!.StatusName ==
                    "Approved by Help Desk")
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

        public async Task<Request?> GetRequestForReview(int id)
        {
            return await _context.Requests
                .Include(r => r.TicketType)
                .Include(r => r.Status)
                .FirstOrDefaultAsync(r =>
                    r.RequestId == id);
        }

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

            if (request.StatusId != 2)
                return null;

            var approval = new RequestApproval
            {
                RequestId = requestId,
                ApprovedBy = "Dummy Supervisor",
                Decision = decision,
                Remarks = remarks ?? "",
                DecisionDate = DateTime.Now
            };

            _context.RequestApprovals.Add(approval);

            if (decision == "Approved")
            {
                request.StatusId = 3;
            }
            else if (decision == "Rejected")
            {
                request.StatusId = 5;
            }

            await _context.SaveChangesAsync();

            return request;
        }
    }
}