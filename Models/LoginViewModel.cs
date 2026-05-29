using System.ComponentModel.DataAnnotations;

namespace InternalRequestSystem.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

    }
}

