using System.ComponentModel.DataAnnotations;

namespace CPMP.Models
{
	public class CreateUserModelView
	{
		[Display(Name = "User name")]
		[Required(ErrorMessage = "User name is required")]
		[StringLength(50, ErrorMessage = "User name cannot be longer than 50 characters")]
		public string Username { get; set; } = null!;
		[Display(Name = "Password")]
		[Required(ErrorMessage = "Password is required")]
		[DataType(DataType.Password)]
		public string Password { get; set; }
		[Display(Name = "Repeat Password")]
		[Required(ErrorMessage = "Repeat Password is required")]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string RepeatedPassword { get; set; }
		public string Email { get; set; } = null!;
	}
}
