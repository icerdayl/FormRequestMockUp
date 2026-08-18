using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RequestForm.Models
{
    public class SubTask
    {
        [Key]
        public int SubTaskId { get; set; }

        public int FeatureId { get; set; }

        // Denormalized on purpose: lets the Dashboard/Gantt query
        // subtasks directly by RequestId without an extra join
        // through Feature. A Feature never moves between Requests,
        // so this never drifts out of sync.
        public int RequestId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = SubTaskStatuses.NotStarted;

        public string? AssignedTo { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? DueDate { get; set; }

        public decimal? EstimatedManDays { get; set; }

        public decimal? ActualManDays { get; set; }

        public DateTime? CompletedDate { get; set; }

        public string? CompletionRemarks { get; set; }

        public string? ResultAttachmentPath { get; set; }

        public int SortOrder { get; set; }

        [ForeignKey(nameof(FeatureId))]
        public Feature? Feature { get; set; }

        [ForeignKey(nameof(RequestId))]
        public Request? Request { get; set; }
    }

    public static class SubTaskStatuses
    {
        public const string NotStarted = "Not Started";
        public const string InProgress = "In Progress";
        public const string Done = "Done";
    }
}