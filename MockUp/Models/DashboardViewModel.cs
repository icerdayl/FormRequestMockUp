namespace RequestForm.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalRequests { get; set; }

        public int PendingRequests { get; set; }

        public int ApprovedRequests { get; set; }

        public int RejectedRequests { get; set; }

        public int InProgressRequests { get; set; }

        public int CompletedRequests { get; set; }

        public List<Request> RecentRequests { get; set; } = new();

        public List<Request> HighPriorityRequests { get; set; } = new();

        // ==========================================
        // DEADLINE INDICATOR
        // ==========================================

        public int OverdueRequests { get; set; }

        public int DueSoonRequests { get; set; }

        // Open (not Completed/Rejected) requests,
        // soonest deadline first
        public List<Request> DeadlineWatch { get; set; } = new();

        // ==========================================
        // GANTT CHART
        // ==========================================

        // One bar per subtask (so man-days actually segment on the
        // chart instead of one bar per whole request), grouped by the
        // developer assigned to the request by Help Desk. Requests with
        // no Features/Subtasks yet fall back to a single request-level
        // bar under their RequestAssignment developer, so older data
        // doesn't just disappear from the chart.
        // "Unassigned" bucket for anything with no current request assignment.
        public Dictionary<string, List<GanttBarItem>> GanttByDeveloper { get; set; } = new();

        // Request count grouped by actual status name,
        // used to draw the status graph
        public Dictionary<string, int> StatusBreakdown { get; set; } = new();
    }

    // A single Gantt bar - either a subtask (the common case) or,
    // as a fallback, a whole request that has no subtasks yet.
    public class GanttBarItem
    {
        public int RequestId { get; set; }

        public string ReferenceNumber { get; set; } = string.Empty;

        public string RequestTitle { get; set; } = string.Empty;

        // Null when this bar represents a whole request rather than
        // one of its subtasks (the no-subtasks-yet fallback case).
        public string? FeatureTitle { get; set; }

        public string? SubTaskTitle { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public decimal? ManDays { get; set; }

        public string Priority { get; set; } = "Medium";

        // "Not Started" / "In Progress" / "Done" for a subtask bar,
        // or the request's own status name for a fallback bar.
        public string Status { get; set; } = string.Empty;
    }
}