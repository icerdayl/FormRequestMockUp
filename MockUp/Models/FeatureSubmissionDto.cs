namespace RequestForm.Models
{
    public class FeatureSubmissionDto
    {
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? AcceptanceCriteria { get; set; }

        public string Priority { get; set; } = "Medium";

        public List<SubTaskSubmissionDto> SubTasks { get; set; } = new();
    }

    public class SubTaskSubmissionDto
    {
        public string Title { get; set; } = string.Empty;

        public DateTime? StartDate { get; set; }

        public DateTime? DueDate { get; set; }

        public decimal? EstimatedManDays { get; set; }
    }
}
