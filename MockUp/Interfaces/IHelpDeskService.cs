using RequestForm.Models;
using RequestForm.Models.ViewModels;

namespace RequestForm.Interfaces
{
    public interface IHelpDeskService
    {

        // FOR SEARCH AND FILTER
        Task<List<Request>> GetRequestList(
            string? search,
            string? status);

        // ASSIGNMENT 

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
    }
}