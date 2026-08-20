using Microsoft.EntityFrameworkCore;
using RequestForm.Data;
using RequestForm.Interfaces;
using RequestForm.Models;

namespace RequestForm.Services
{
    public class SubTaskService : ISubTaskService
    {
        private readonly ApplicationDbContext _context;

        public SubTaskService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SubTask>> GetByFeatureId(int featureId)
        {
            return await _context.SubTasks
                .Where(s => s.FeatureId == featureId)
                .OrderBy(s => s.SortOrder)
                .ToListAsync();
        }

        public async Task<SubTask?> GetById(int subTaskId)
        {
            return await _context.SubTasks
                .Include(s => s.Feature)
                .FirstOrDefaultAsync(s => s.SubTaskId == subTaskId);
        }

        public async Task<SubTask> Create(SubTask subTask)
        {
            var feature = await _context.Features
                .FirstOrDefaultAsync(f => f.FeatureId == subTask.FeatureId);

            if (feature == null)
                throw new InvalidOperationException("Feature not found.");

            // Keep the denormalized RequestId in sync with its
            // parent Feature - never trust the posted value.
            subTask.RequestId = feature.RequestId;

            var maxSortOrder = await _context.SubTasks
                .Where(s => s.FeatureId == subTask.FeatureId)
                .Select(s => (int?)s.SortOrder)
                .MaxAsync();

            subTask.SortOrder = (maxSortOrder ?? 0) + 1;
            subTask.Status = SubTaskStatuses.NotStarted;

            _context.SubTasks.Add(subTask);

            await _context.SaveChangesAsync();

            return subTask;
        }

        public async Task<bool> ToggleDone(int subTaskId)
        {
            var subTask = await _context.SubTasks
                .FirstOrDefaultAsync(s => s.SubTaskId == subTaskId);

            if (subTask == null)
                return false;

            if (subTask.Status == SubTaskStatuses.Done)
            {
                subTask.Status = SubTaskStatuses.NotStarted;
                subTask.CompletedDate = null;
            }
            else
            {
                subTask.Status = SubTaskStatuses.Done;
                subTask.CompletedDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            await RecalculateRequestStatus(subTask.RequestId);

            return true;
        }

        public async Task<bool> UpdateStatus(
            int subTaskId,
            string status,
            string? completionRemarks,
            decimal? actualManDays,
            string? resultAttachmentPath)
        {
            var subTask = await _context.SubTasks
                .FirstOrDefaultAsync(s => s.SubTaskId == subTaskId);

            if (subTask == null)
                return false;

            subTask.Status = status;

            if (status == SubTaskStatuses.Done)
            {
                subTask.CompletedDate = DateTime.Now;

                if (completionRemarks != null)
                    subTask.CompletionRemarks = completionRemarks;

                if (actualManDays.HasValue)
                    subTask.ActualManDays = actualManDays;

                if (!string.IsNullOrWhiteSpace(resultAttachmentPath))
                    subTask.ResultAttachmentPath = resultAttachmentPath;
            }
            else
            {
                subTask.CompletedDate = null;
            }

            await _context.SaveChangesAsync();

            await RecalculateRequestStatus(subTask.RequestId);

            return true;
        }

        // Status IDs: 4 = "Approved by Manager", 6 = "In Progress",
        // 7 = "Completed" (see ApplicationDbContext seed data).
        // Only requests already at "Approved by Manager" or already
        // auto-promoted to "In Progress" are managed here - a
        // Request that's Pending, Rejected, or still earlier in
        // approval is never touched by Subtask activity.
        private async Task RecalculateRequestStatus(int requestId)
        {
            var request = await _context.Requests
                .FirstOrDefaultAsync(r => r.RequestId == requestId);

            if (request == null)
                return;

            if (request.StatusId != 4 && request.StatusId != 6)
                return;

            var subTasks = await _context.SubTasks
                .Where(s => s.RequestId == requestId)
                .ToListAsync();

            if (subTasks.Count == 0)
                return;

            var allDone = subTasks.All(s => s.Status == SubTaskStatuses.Done);
            var anyStarted = subTasks.Any(s => s.Status != SubTaskStatuses.NotStarted);

            if (allDone)
            {
                request.StatusId = 7; // Completed
            }
            else if (anyStarted && request.StatusId == 4)
            {
                request.StatusId = 6; // In Progress
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> Delete(int subTaskId)
        {
            var subTask = await _context.SubTasks
                .FirstOrDefaultAsync(s => s.SubTaskId == subTaskId);

            if (subTask == null)
                return false;

            _context.SubTasks.Remove(subTask);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}