using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RequestForm.Models
{
    public class Request
    {
        [Key]
        public int RequestId { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public int RequestTypeId { get; set; }

        [MaxLength(30)]
        public string ReferenceNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Department { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Priority { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public DateTime PreferredCompletionDate { get; set; }

        public string? AttachmentPath { get; set; }

        //public string? HelpDeskRemarks { get; set; }

        public int StatusId { get; set; }

        public DateTime DateSubmitted { get; set; }

        [ForeignKey(nameof(RequestTypeId))]
        public RequestType? RequestType { get; set; }

        [ForeignKey(nameof(StatusId))]
        public Status? Status { get; set; }

        public ICollection<RequestAssignment> RequestAssignments { get; set; }
            = new List<RequestAssignment>();

        public ICollection<RequestApproval> RequestApprovals { get; set; }
            = new List<RequestApproval>();
    }
}