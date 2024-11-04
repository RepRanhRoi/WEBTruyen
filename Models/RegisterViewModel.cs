using System.ComponentModel.DataAnnotations;

namespace WebTruyen.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
    }
}
