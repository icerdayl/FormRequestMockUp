using RequestForm.Models;

namespace RequestForm.Interfaces
{
    public interface IFeatureService
    {
        Task<List<Feature>> GetByRequestId(int requestId);

        Task<Feature?> GetById(int featureId);

        Task<Feature> Create(Feature feature);

        // Creates a set of Features (each with its Subtasks) for a
        // freshly-created Request in one transaction — used by the
        // Create Request form's inline Features & Subtasks builder.
        Task CreateBatchForRequest(int requestId, List<FeatureSubmissionDto> features);

        Task<bool> Delete(int featureId);
    }
}