using System.ComponentModel.DataAnnotations;

namespace RequestForm.Models
{
    public class RequestType
    {
        [Key]
        public int RequestTypeId { get; set; }

        [Required]
        [MaxLength(100)]
        public string RequestTypeName { get; set; } = string.Empty;
    }
}