using RequestForm.Models;

namespace RequestForm.Interfaces
{
    public interface ISupervisorService
    {
        Task<List<Request>> GetPendingApprovals(
            string? search,
            string? status);

        Task<Request?> GetRequestForReview(int id);

        Task<Request?> ProcessApproval(
            int requestId,
            string decision,
            string remarks);
    }
}