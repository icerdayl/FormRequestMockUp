using RequestForm.Models;

namespace RequestForm.Interfaces
{
    public interface IManagerService
    {
        Task<List<Request>> GetPendingApprovals(
            string? search);

        Task<Request?> GetRequestForReview(
            int id);

        Task<Request?> ProcessApproval(
            int requestId,
            string decision,
            string remarks);
    }
}