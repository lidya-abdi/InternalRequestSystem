using System.ComponentModel.DataAnnotations;

namespace InternalRequestSystem.Models
{
    public class Request
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Request type is required")]
        public string RequestType { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string SubmittedByName { get; set; } = string.Empty;
        public string SubmittedByEmail { get; set; } = string.Empty;

        // Foreign key for Department association (Department 1 ---- * Requests)

        [Range(1, int.MaxValue, ErrorMessage = "Please select a department")]
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        public int? UserId { get; set; }
        public AppUser? User { get; set; }

        public List<Approval> Approvals { get; set; } = new List<Approval>();
    }
}
