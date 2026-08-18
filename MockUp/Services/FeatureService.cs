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

                    _context.SubTasks.Add(new SubTask
                    {
                        FeatureId = feature.FeatureId,
                        RequestId = requestId,
                        Title = subTaskDto.Title,
                        AssignedTo = subTaskDto.AssignedTo,
                        StartDate = subTaskDto.StartDate,
                        DueDate = subTaskDto.DueDate,
                        EstimatedManDays = subTaskDto.EstimatedManDays,
                        Status = SubTaskStatuses.NotStarted,
                        SortOrder = sortOrder
                    });

                    sortOrder++;
                }

                await _context.SaveChangesAsync();
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