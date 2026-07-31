using System.ComponentModel.DataAnnotations;

namespace RequestForm.Models
{
    public class Status
    {
        [Key]
        public int StatusId { get; set; }

        [Required]
        [MaxLength(50)]
        public string StatusName { get; set; } = string.Empty;
    }
}