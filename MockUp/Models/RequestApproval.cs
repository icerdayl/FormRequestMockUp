using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RequestForm.Models
{
    public class RequestApproval
    {
        [Key]
        public int ApprovalId { get; set; }

        public int RequestId { get; set; }

        public string ApprovedBy { get; set; } = "";

        public string Decision { get; set; } = "";

        public string Remarks { get; set; } = "";

        public DateTime DecisionDate { get; set; }

        [ForeignKey(nameof(RequestId))]
        public Request? Request { get; set; }
    }
}