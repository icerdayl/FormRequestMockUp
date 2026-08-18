using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RequestForm.Models
{
    public class Feature
    {
        [Key]
        public int FeatureId { get; set; }

        public int RequestId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        // Free text - commonly written as a user story narrative
        // ("As a [role], I want [goal] so that [benefit]."), but
        // the entity itself is a Feature, not that narrative.
        public string? Description { get; set; }

        public string? AcceptanceCriteria { get; set; }

        [MaxLength(20)]
        public string Priority { get; set; } = "Medium";

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [ForeignKey(nameof(RequestId))]
        public Request? Request { get; set; }

        public ICollection<SubTask> SubTasks { get; set; }
            = new List<SubTask>();
    }
}