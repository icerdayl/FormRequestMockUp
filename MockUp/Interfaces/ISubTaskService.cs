using RequestForm.Models;

namespace RequestForm.Interfaces
{
    public interface ISubTaskService
    {
        Task<List<SubTask>> GetByFeatureId(int featureId);

        Task<SubTask?> GetById(int subTaskId);

        Task<SubTask> Create(SubTask subTask);

        Task<bool> ToggleDone(int subTaskId);

        // Full 3-state status update used by the Developer workflow
        // page — also re-evaluates and auto-updates the parent
        // Request's status (Approved -> In Progress -> Completed)
        // based on the combined status of all its Subtasks.
        Task<bool> UpdateStatus(
            int subTaskId,
            string status,
            string? completionRemarks,
            decimal? actualManDays,
            string? resultAttachmentPath);

        Task<bool> Delete(int subTaskId);
    }
}