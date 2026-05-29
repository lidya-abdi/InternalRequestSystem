using System.ComponentModel.DataAnnotations;

namespace InternalRequestSystem.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = string.Empty;
    }
}

