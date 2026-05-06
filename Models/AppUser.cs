using System.ComponentModel.DataAnnotations;

namespace InternalRequestSystem.Models
{
    public class AppUser
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; } = string.Empty;

        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        public int RoleId { get; set; }
        public Role? Role { get; set; }

        public List<Request> Requests { get; set; } = new List<Request>();

    }
}
