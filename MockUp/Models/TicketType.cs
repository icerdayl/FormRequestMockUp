using System.ComponentModel.DataAnnotations;

namespace RequestForm.Models
{
    public class TicketType
    {
        [Key]
        public int TicketTypeId { get; set; }

        [Required]
        [MaxLength(100)]
        public string TicketTypeName { get; set; } = string.Empty;
    }
}