using System.ComponentModel.DataAnnotations;

namespace CPMP.Models
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(50, ErrorMessage = "Username cannot be longer than 50 characters.")]
        public string Username { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage ="Password is not the same")]
        public string ConfirmPassword { get; set; }

    }
}
