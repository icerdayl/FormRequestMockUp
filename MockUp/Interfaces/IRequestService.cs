using RequestForm.Models;

namespace RequestForm.Interfaces
{
    public interface IRequestService
    {
        Task<Request?> GetById(int id);

        Task<List<Request>> GetMyRequests(
            string? search,
            string? status);

        Task Create(Request request);

        Task Update(Request request);

        Task Delete(int id);
    }
}