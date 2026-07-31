using RequestForm.Models;

namespace RequestForm.Interfaces
{
    public interface IRequestService
    {
        Task<List<Request>> GetAll();

        Task<Request?> GetById(int id);

        Task Create(Request request);

        Task Update(Request request);

        Task Delete(int id);
    }
}