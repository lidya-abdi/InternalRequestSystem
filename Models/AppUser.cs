
namespace InternalRequestSystem.Models
{
    public class AppUser
    {
        public int Id { get; set; }

        
        public string? FullName { get; set; } = string.Empty;

        public string? Email { get; set; } = string.Empty;

        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        public int RoleId { get; set; }
        public Role? Role { get; set; }

        public List<Request> Requests { get; set; } = new List<Request>();

        public List<Approval> Approvals { get; set; } = new List<Approval>();

    }
}
