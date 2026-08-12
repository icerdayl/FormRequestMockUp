using Microsoft.EntityFrameworkCore;
using RequestForm.Data;
using RequestForm.Interfaces;
using RequestForm.Models.ViewModels;

namespace RequestForm.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardViewModel> GetDashboard()
        {
            var dashboard = new DashboardViewModel();

            dashboard.TotalRequests =
                await _context.Requests.CountAsync();

            dashboard.PendingRequests =
                await _context.Requests.CountAsync(x => x.Status!.StatusName == "Pending");

            dashboard.ApprovedRequests =
                await _context.Requests.CountAsync(x =>
                    x.Status!.StatusName == "Approved by Help Desk" ||
                    x.Status!.StatusName == "Approved by Supervisor" ||
                    x.Status!.StatusName == "Approved by Manager");

            dashboard.RejectedRequests =
                await _context.Requests.CountAsync(x => x.Status!.StatusName == "Rejected");

            dashboard.InProgressRequests =
                await _context.Requests.CountAsync(x => x.Status!.StatusName == "In Progress");

            dashboard.CompletedRequests =
                await _context.Requests.CountAsync(x => x.Status!.StatusName == "Completed");

            dashboard.RecentRequests =
                await _context.Requests
                    .Include(x => x.Status)
                    .Include(x => x.RequestType)
                    .OrderByDescending(x => x.DateSubmitted)
                    .Take(5)
                    .ToListAsync();

            dashboard.HighPriorityRequests =
                await _context.Requests
                    .Include(x => x.Status)
                    .Include(x => x.RequestType)
                    .Where(x => x.Priority == "High" &&
                         x.Status.StatusName != "Completed")
                    .OrderByDescending(x => x.DateSubmitted)
                    .ToListAsync();


            // ==========================================
            // DEADLINE INDICATOR
            // ==========================================

            var today = DateTime.Today;

            var openRequests = await _context.Requests
                .Include(x => x.Status)
                .Include(x => x.RequestType)
                .Where(x =>
                    x.Status!.StatusName != "Completed" &&
                    x.Status!.StatusName != "Rejected")
                .ToListAsync();

            dashboard.OverdueRequests =
                openRequests.Count(x =>
                    x.PreferredCompletionDate.Date < today);

            dashboard.DueSoonRequests =
                openRequests.Count(x =>
                    x.PreferredCompletionDate.Date >= today &&
                    x.PreferredCompletionDate.Date <= today.AddDays(3));

            dashboard.DeadlineWatch = openRequests
                .OrderBy(x => x.PreferredCompletionDate)
                .Take(10)
                .ToList();


            // ==========================================
            // TIMELINE / GRAPH
            // ==========================================

            dashboard.TimelineTasks = openRequests
                .Where(x => x.StartDate.HasValue)
                .OrderBy(x => x.StartDate)
                .Take(15)
                .ToList();

            dashboard.StatusBreakdown = await _context.Requests
                .Include(x => x.Status)
                .GroupBy(x => x.Status!.StatusName)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Status, g => g.Count);

            return dashboard;
        }
    }
}