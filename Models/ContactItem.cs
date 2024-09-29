
using System.ComponentModel.DataAnnotations;

namespace CarDealer.Models
{
    public class ContactItem
    {
        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress]
        public string EmailAddress { get; set; } = String.Empty;
        public string? Subject { get; set; }

        [Required(ErrorMessage = "Message body can't be empty")]
        public string Message { get; set; } = String.Empty;
    }
}
