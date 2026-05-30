using System.ComponentModel.DataAnnotations;

namespace InternalRequestSystem.Models
{
    public class Department
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Department name is required.")]
        public string DepartmentName { get; set; } = string.Empty;

        public List<AppUser> Users { get; set; } = new List<AppUser>();

        public List<Request> Requests { get; set; } = new List<Request>();
    }
}
