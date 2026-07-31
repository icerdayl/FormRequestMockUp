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
    }
}