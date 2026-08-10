using RequestForm.Models;
using RequestForm.Models.ViewModels;

namespace RequestForm.Interfaces
{
    public interface IHelpDeskService
    {
       
        Task<List<Request>> GetRequestList(
            string? search,
            string? status);

        Task<AssignmentPageViewModel> GetAssignments(
            string? search,
            string? status);

        Task<bool> AssignDeveloper(
            AssignmentViewModel model);

        Task<bool> UpdateStatus(
            int requestId,
            string status,
            string remarks);

        Task<Request?> GetRequestForReview(int id);

        Task<RequestApproval?> GetLatestApproval(int requestId);
    }
}