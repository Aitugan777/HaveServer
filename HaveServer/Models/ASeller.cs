using System.ComponentModel.DataAnnotations;

namespace AitukServer.Models
{
    public class ASeller
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public string Password { get; set; }

        public string FullName { get; set; }
    }
}
