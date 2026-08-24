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
                    .Include(x => x.TicketType)
                    .OrderByDescending(x => x.DateSubmitted)
                    .Take(5)
                    .ToListAsync();

            dashboard.HighPriorityRequests =
                await _context.Requests
                    .Include(x => x.Status)
                    .Include(x => x.TicketType)
                    .Where(x => x.Priority == "High" &&
                         x.Status!.StatusName != "Completed")
                    .OrderByDescending(x => x.DateSubmitted)
                    .ToListAsync();


            // ==========================================
            // DEADLINE INDICATOR
            // ==========================================

            var today = DateTime.Today;

            var openRequests = await _context.Requests
                .Include(x => x.Status)
                .Include(x => x.TicketType)
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
            // GANTT CHART (one bar per subtask, grouped by the
            // developer assigned to the request by Help Desk -
            // this is what makes man-days actually segment on the chart
            // without allowing per-subtask developer assignment)
            // ==========================================

            var scheduledRequests = await _context.Requests
                .Include(x => x.Status)
                .Include(x => x.RequestAssignments)
                .Include(x => x.Features)
                    .ThenInclude(f => f.SubTasks)
                .Where(x =>
                    x.StartDate.HasValue &&
                    ( 
                        x.Status!.StatusName == "Approved by Manager" ||
                        x.Status!.StatusName == "In Progress" ||
                        x.Status!.StatusName == "Completed"
                    ))
                .ToListAsync();

            var ganttBars = new List<(string Developer, GanttBarItem Bar)>();

            foreach (var request in scheduledRequests)
            {
                var allSubTasks = request.Features
                    .SelectMany(f => f.SubTasks.Select(s => (Feature: f, SubTask: s)))
                    .Where(x => x.SubTask.StartDate.HasValue && x.SubTask.DueDate.HasValue)
                    .ToList();

                if (allSubTasks.Any())
                {
                    var currentAssignment = request.RequestAssignments
                        .FirstOrDefault(a => a.IsCurrent);

                    foreach (var (feature, subTask) in allSubTasks)
                    {
                        ganttBars.Add((
                            currentAssignment?.AssignedTo ?? "Unassigned",
                            new GanttBarItem
                            {
                                RequestId = request.RequestId,
                                ReferenceNumber = request.ReferenceNumber,
                                RequestTitle = request.Title,
                                FeatureTitle = feature.Title,
                                SubTaskTitle = subTask.Title,
                                Start = subTask.StartDate!.Value,
                                End = subTask.DueDate!.Value,
                                ManDays = subTask.EstimatedManDays,
                                Priority = request.Priority,
                                Status = subTask.Status
                            }));
                    }
                }
                // No subtask fallback: the Gantt is intentionally
                // segmented only by actual subtasks and their man-days.
                // Requests with no dated subtasks remain absent until
                // their subtasks are scheduled.

            }

            dashboard.GanttByDeveloper = ganttBars
                .GroupBy(x => x.Developer)
                .OrderBy(g => g.Key == "Unassigned" ? 1 : 0)
                .ThenBy(g => g.Key)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.Bar).OrderBy(b => b.Start).ToList());

            dashboard.StatusBreakdown = await _context.Requests
                .Include(x => x.Status)
                .GroupBy(x => x.Status!.StatusName)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Status, g => g.Count);

            return dashboard;
        }
    }
}