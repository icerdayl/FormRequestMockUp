using RequestForm.Models;

namespace RequestForm.Interfaces
{
    public interface ISubTaskService
    {
        Task<List<SubTask>> GetByFeatureId(int featureId);

        Task<SubTask?> GetById(int subTaskId);

        Task<SubTask> Create(SubTask subTask);

        Task<bool> ToggleDone(int subTaskId);

        Task<bool> Delete(int subTaskId);
    }
}