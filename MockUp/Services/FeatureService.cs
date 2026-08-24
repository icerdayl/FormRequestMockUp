using Microsoft.EntityFrameworkCore;
using RequestForm.Data;
using RequestForm.Interfaces;
using RequestForm.Models;

namespace RequestForm.Services
{
    public class FeatureService : IFeatureService
    {
        private readonly ApplicationDbContext _context;

        public FeatureService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Feature>> GetByRequestId(int requestId)
        {
            return await _context.Features
                .Include(f => f.SubTasks)
                .Where(f => f.RequestId == requestId)
                .OrderBy(f => f.CreatedDate)
                .ToListAsync();
        }

        public async Task<Feature?> GetById(int featureId)
        {
            return await _context.Features
                .Include(f => f.Request)
                .Include(f => f.SubTasks)
                .FirstOrDefaultAsync(f => f.FeatureId == featureId);
        }

        public async Task<Feature> Create(Feature feature)
        {
            feature.CreatedDate = DateTime.Now;

            _context.Features.Add(feature);

            await _context.SaveChangesAsync();

            return feature;
        }

        public async Task CreateBatchForRequest(
            int requestId,
            List<FeatureSubmissionDto> features)
        {
            if (features == null || features.Count == 0)
                return;

            using var transaction = await _context.Database.BeginTransactionAsync();

            var request = await _context.Requests
                .FirstOrDefaultAsync(r => r.RequestId == requestId);

            if (request == null)
                throw new InvalidOperationException("Parent request not found.");

            var today = DateTime.Today;
            var latestSubTaskDueDate = (DateTime?)null;

            foreach (var featureDto in features)
            {
                if (string.IsNullOrWhiteSpace(featureDto.Title))
                    continue;

                var feature = new Feature
                {
                    RequestId = requestId,
                    Title = featureDto.Title,
                    Description = featureDto.Description,
                    AcceptanceCriteria = featureDto.AcceptanceCriteria,
                    Priority = string.IsNullOrWhiteSpace(featureDto.Priority)
                        ? "Medium"
                        : featureDto.Priority,
                    CreatedDate = DateTime.Now
                };

                _context.Features.Add(feature);

                // Save now so feature.FeatureId is populated for
                // its subtasks below.
                await _context.SaveChangesAsync();

                var sortOrder = 1;

                foreach (var subTaskDto in featureDto.SubTasks)
                {
                    if (string.IsNullOrWhiteSpace(subTaskDto.Title))
                        continue;

                    if (!subTaskDto.StartDate.HasValue || !subTaskDto.DueDate.HasValue)
                        throw new InvalidOperationException($"Subtask '{subTaskDto.Title}' must have both a start date and a due date.");

                    if (subTaskDto.StartDate.Value.Date < today)
                        throw new InvalidOperationException($"Subtask '{subTaskDto.Title}' cannot have a start date earlier than today.");

                    if (subTaskDto.DueDate.Value.Date < today)
                        throw new InvalidOperationException($"Subtask '{subTaskDto.Title}' cannot have a due date earlier than today.");

                    if (subTaskDto.DueDate.Value.Date < subTaskDto.StartDate.Value.Date)
                        throw new InvalidOperationException($"Subtask '{subTaskDto.Title}' cannot have a due date earlier than its start date.");

                    if (request.StartDate.HasValue &&
                        subTaskDto.StartDate.Value.Date < request.StartDate.Value.Date)
                    {
                        throw new InvalidOperationException($"Subtask '{subTaskDto.Title}' cannot start before the request start date.");
                    }

                    var manDays =
                        (decimal)(subTaskDto.DueDate.Value.Date - subTaskDto.StartDate.Value.Date).TotalDays + 1m;

                    latestSubTaskDueDate = !latestSubTaskDueDate.HasValue ||
                                           subTaskDto.DueDate.Value.Date > latestSubTaskDueDate.Value
                        ? subTaskDto.DueDate.Value.Date
                        : latestSubTaskDueDate.Value;

                    _context.SubTasks.Add(new SubTask
                    {
                        FeatureId = feature.FeatureId,
                        RequestId = requestId,
                        Title = subTaskDto.Title,
                        StartDate = subTaskDto.StartDate.Value.Date,
                        DueDate = subTaskDto.DueDate.Value.Date,
                        EstimatedManDays = manDays,
                        Status = SubTaskStatuses.NotStarted,
                        SortOrder = sortOrder
                    });

                    sortOrder++;
                }

                await _context.SaveChangesAsync();
            }

            if (latestSubTaskDueDate.HasValue &&
                latestSubTaskDueDate.Value != request.PreferredCompletionDate.Date)
            {
                throw new InvalidOperationException(
                    $"The request Completion Date must match the latest subtask due date ({latestSubTaskDueDate:MMMM dd, yyyy}).");
            }

            await transaction.CommitAsync();
        }

        public async Task<bool> Delete(int featureId)
        {
            var feature = await _context.Features
                .FirstOrDefaultAsync(f => f.FeatureId == featureId);

            if (feature == null)
                return false;

            _context.Features.Remove(feature);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}