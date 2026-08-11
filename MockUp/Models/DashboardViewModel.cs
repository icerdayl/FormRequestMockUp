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
        // TIMELINE / GRAPH
        // ==========================================

        // Open requests that have a StartDate set,
        // used to draw the task timeline
        public List<Request> TimelineTasks { get; set; } = new();

        // Request count grouped by actual status name,
        // used to draw the status graph
        public Dictionary<string, int> StatusBreakdown { get; set; } = new();
    }
}