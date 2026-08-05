using RequestForm.Models;

namespace RequestForm.Models.ViewModels
{
    public class AssignmentPageViewModel
    {
        public List<Request> ApprovedRequests { get; set; } = new();

        public List<Request> AssignedRequests { get; set; } = new();
    }
}