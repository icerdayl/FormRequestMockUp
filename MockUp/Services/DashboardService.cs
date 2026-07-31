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
                await _context.Requests.CountAsync(x => x.StatusId == 1);

            dashboard.ApprovedRequests =
                await _context.Requests.CountAsync(x => x.StatusId == 2);

            dashboard.RejectedRequests =
                await _context.Requests.CountAsync(x => x.StatusId == 3);

            dashboard.InProgressRequests =
                await _context.Requests.CountAsync(x => x.StatusId == 4);

            dashboard.CompletedRequests =
                await _context.Requests.CountAsync(x => x.StatusId == 5);

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

            return dashboard;
        }
    }
}