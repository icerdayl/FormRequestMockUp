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

            return true;
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