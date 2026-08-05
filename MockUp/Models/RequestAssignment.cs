using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RequestForm.Models
{
    public class RequestAssignment
    {
        [Key]
        public int AssignmentId { get; set; }

        public int RequestId { get; set; }

        [Required]
        public string AssignedTo { get; set; } = "";

        [Required]
        public string AssignedBy { get; set; } = "";

        public DateTime AssignedDate { get; set; }

        public bool IsCurrent { get; set; } = true;

        [ForeignKey(nameof(RequestId))]
        public Request? Request { get; set; }


    }


}